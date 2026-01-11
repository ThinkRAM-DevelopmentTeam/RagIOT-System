using System;
using System.Net;
using System.Threading.Tasks;
using IotRagFunctionApp.Models;
using IotRagFunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IotRagFunctionApp.Functions
{
    public class ChatWithRag
    {
        private readonly IRagService _rag;

        public ChatWithRag(IRagService rag)
        {
            _rag = rag;
        }

        [Function("ChatWithRag")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chat")] HttpRequestData req, FunctionContext context)
        {
            var logger = context.GetLogger("ChatWithRag");
            try
            {
                string requestBody = await req.ReadAsStringAsync();
                var chatRequest = JsonConvert.DeserializeObject<ChatRequest>(requestBody);

                if (chatRequest == null || string.IsNullOrEmpty(chatRequest.DeviceId) || string.IsNullOrEmpty(chatRequest.Question))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new { error = "deviceId and question are required" });
                    return badResponse;
                }

                logger.LogInformation("Chat query for device {device}: {question}", chatRequest.DeviceId, chatRequest.Question);
                var response = await _rag.QueryWithRagAsync(chatRequest.DeviceId, chatRequest.Question);

var httpResponse = req.CreateResponse(HttpStatusCode.OK);
                var jsonString = JsonConvert.SerializeObject(response);
                await httpResponse.WriteStringAsync(jsonString);
                httpResponse.Headers.Remove("Content-Type");
                httpResponse.Headers.Add("Content-Type", "application/json");
                return httpResponse;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in ChatWithRag: {ex.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteAsJsonAsync(new { error = ex.Message });
                return errorResponse;
            }
        }
    }
}
