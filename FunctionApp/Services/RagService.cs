using IotRagFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IotRagFunctionApp.Services
{
    public interface IRagService
    {
        Task<ChatResponse> QueryWithRagAsync(string deviceId, string question);
    }

    public class RagService : IRagService
    {
        private readonly ICosmosService _cosmos;
        private readonly IEmbeddingService _embedding;

        public RagService(ICosmosService cosmos, IEmbeddingService embedding)
        {
            _cosmos = cosmos;
            _embedding = embedding;
        }

        public async Task<ChatResponse> QueryWithRagAsync(string deviceId, string question)
        {
            // If Cosmos is not initialized, return mock response for demo
            if (_cosmos == null)
            {
                return GetMockResponse(deviceId, question);
            }

            try
            {
                var questionEmbedding = await _embedding.GenerateEmbeddingAsync(question);
                var relevantDocs = await _cosmos.VectorSearchAsync(questionEmbedding, topK: 5);
                var recentTelemetry = await _cosmos.GetRecentTelemetryAsync(deviceId, minutes: 120);
                var context = BuildContext(relevantDocs, recentTelemetry, question);
                var answer = GenerateMockAnswer(context, question, relevantDocs, recentTelemetry);

                return new ChatResponse
                {
                    Answer = answer,
                    Sources = relevantDocs.Select(d => $"{d.SourceDoc} (chunk {d.ChunkIndex})").ToList(),
                    RecentTelemetry = recentTelemetry.FirstOrDefault()
                };
            }
            catch (Exception)
            {
                // Fallback to mock response if Cosmos operations fail
                return GetMockResponse(deviceId, question);
            }
        }

        private ChatResponse GetMockResponse(string deviceId, string question)
        {
            var mockTelemetry = new DeviceTelemetry
            {
                DeviceId = deviceId,
                TemperatureC = 78,
                VibrationMm = 3.2,
                IsAnomaly = question.ToLower().Contains("temperature") || question.ToLower().Contains("high"),
                Timestamp = DateTime.UtcNow.AddMinutes(-5)
            };

            string mockAnswer = GenerateMockAnswerForDevice(deviceId, question);

            return new ChatResponse
            {
                Answer = mockAnswer,
                Sources = new List<string> 
                { 
                    "Pump-Maintenance-SOP.pdf (chunk 2)",
                    "Equipment-Safety-Manual.pdf (chunk 5)",
                    "Troubleshooting-Guide.pdf (chunk 1)"
                },
                RecentTelemetry = mockTelemetry
            };
        }

        private string GenerateMockAnswerForDevice(string deviceId, string question)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"**Analysis for {deviceId}:**\n");

            if (question.ToLower().Contains("temperature") || question.ToLower().Contains("high"))
            {
                sb.AppendLine("⚠️ **High Temperature Detected**");
                sb.AppendLine("- Current temperature: 78°C (elevated above normal 65°C)");
                sb.AppendLine("- **Recommended Actions:**");
                sb.AppendLine("  1. Check coolant levels and refill if necessary");
                sb.AppendLine("  2. Verify pump impeller for blockages");
                sb.AppendLine("  3. Clean or replace inlet filter");
                sb.AppendLine("  4. Check bearing temperature sensors");
                sb.AppendLine("- **Reference:** Pump Maintenance SOP Section 4.2 - Temperature Management");
            }
            else if (question.ToLower().Contains("vibration") || question.ToLower().Contains("vibrate"))
            {
                sb.AppendLine("📊 **Vibration Analysis**");
                sb.AppendLine("- Current vibration: 3.2mm (within normal range)");
                sb.AppendLine("- **Status:** Operating normally");
                sb.AppendLine("- **Preventive Measures:**");
                sb.AppendLine("  1. Continue routine lubrication schedule");
                sb.AppendLine("  2. Monitor for increased vibration trends");
                sb.AppendLine("  3. Perform alignment check during next maintenance window");
            }
            else if (question.ToLower().Contains("procedure") || question.ToLower().Contains("safety"))
            {
                sb.AppendLine("🔒 **Safety Procedures**");
                sb.AppendLine("- **Lockout/Tagout:** Always lock the device before maintenance");
                sb.AppendLine("- **Pressure Relief:** Depressurize system per SOP Section 2.1");
                sb.AppendLine("- **Personal Protection:** Safety glasses, gloves, and steel-toed boots required");
                sb.AppendLine("- **Emergency Stop:** Locate emergency stop button on main panel");
            }
            else if (question.ToLower().Contains("troubleshoot"))
            {
                sb.AppendLine("🔧 **Troubleshooting Guide**");
                sb.AppendLine("- Device: " + deviceId);
                sb.AppendLine("- Current Status: Normal Operation");
                sb.AppendLine("- **Common Issues:**");
                sb.AppendLine("  1. Insufficient flow → Check inlet filter");
                sb.AppendLine("  2. High noise → Verify liquid levels");
                sb.AppendLine("  3. Temperature alarm → Review cooling system");
                sb.AppendLine("- **Next Steps:** Contact maintenance team if issue persists");
            }
            else
            {
                sb.AppendLine($"**General Status for {deviceId}:**");
                sb.AppendLine("- Temperature: 78°C");
                sb.AppendLine("- Vibration: 3.2mm");
                sb.AppendLine("- Status: Operational");
                sb.AppendLine("- Last maintenance: 30 days ago");
                sb.AppendLine("\nFor specific maintenance procedures, refer to the Equipment Maintenance Manual.");
            }

            return sb.ToString();
        }

        private string BuildContext(List<Document> docs, List<DeviceTelemetry> telemetry, string question)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("## Relevant Documentation");
            foreach (var doc in docs)
            {
                sb.AppendLine($"From {doc.SourceDoc}: {doc.ChunkText}");
                sb.AppendLine();
            }

            sb.AppendLine("## Recent Device Telemetry");
            if (telemetry.Any())
            {
                var latest = telemetry.First();
                sb.AppendLine($"Latest reading: Temp={latest.TemperatureC}°C, Vibration={latest.VibrationMm}mm, Anomaly={latest.IsAnomaly}");
            }

            sb.AppendLine($"\n## User Question: {question}");
            return sb.ToString();
        }

        private string GenerateMockAnswer(string context, string question, List<Document> docs, List<DeviceTelemetry> telemetry)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Based on the operational manuals and current telemetry:");
            sb.AppendLine();
            if (telemetry.Any())
            {
                var latest = telemetry.First();
                if (latest.IsAnomaly)
                {
                    sb.AppendLine($"⚠️ **ALERT**: Device has detected anomalies. Temperature is elevated at {latest.TemperatureC}°C with vibration at {latest.VibrationMm}mm.");
                }
                else
                {
                    sb.AppendLine($"✅ Device operating normally. Current temperature: {latest.TemperatureC}°C, Vibration: {latest.VibrationMm}mm.");
                }
            }

            if (docs.Any())
            {
                sb.AppendLine();
                sb.AppendLine("**Relevant Maintenance Guidance:**");
                foreach (var doc in docs.Take(2))
                {
                    sb.AppendLine($"- {doc.ChunkText.Substring(0, Math.Min(100, doc.ChunkText.Length))}...");
                }
            }

            sb.AppendLine();
            sb.AppendLine("For detailed troubleshooting, consult the full maintenance manuals.");
            return sb.ToString();
        }
    }
}
