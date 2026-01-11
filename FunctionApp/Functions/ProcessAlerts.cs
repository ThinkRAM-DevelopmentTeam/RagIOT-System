using System.Threading.Tasks;
using IotRagFunctionApp.Models;
using IotRagFunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System;

namespace IotRagFunctionApp.Functions
{
    public class ProcessAlerts
    {
        private readonly ICosmosService _cosmos;
        public ProcessAlerts(ICosmosService cosmos)
        {
            _cosmos = cosmos;
        }

        // TEMPORARILY DISABLED - Service Bus emulator connection issues  
        /*
        [Function("ProcessAlerts")]
        public async Task Run([ServiceBusTrigger("alerts-queue", Connection = "ServiceBusConnection")] string message, FunctionContext context)
        {
            var logger = context.GetLogger("ProcessAlerts");
            try
            {
                var alert = JsonSerializer.Deserialize<Alert>(message);
                if (alert == null)
                {
                    logger.LogWarning("Invalid alert message");
                    return;
                }

                await _cosmos.UpsertAlertAsync(alert);
                logger.LogInformation("Processed alert for {device}: {reason}", alert.DeviceId, alert.Reason);

                // Optionally update device state
                var device = await _cosmos.GetDeviceAsync(alert.DeviceId);
                device.Status = "alert";
                device.LastAlertReason = alert.Reason;
                device.LastAlertTime = alert.Timestamp;
                await _cosmos.UpsertDeviceAsync(device);
                logger.LogInformation("Updated device {device} status to alert", device.DeviceId);

                // Build combined alert payload and forward to combined-alerts-queue if available
                try
                {
                    var recentTelemetry = await _cosmos.GetRecentTelemetryAsync(alert.DeviceId, minutes: 30);
                    var combined = new {
                        alertId = alert.Id,
                        deviceId = alert.DeviceId,
                        reason = alert.Reason,
                        timestamp = alert.Timestamp,
                        deviceStatus = device.Status,
                        latestTelemetry = recentTelemetry.Count > 0 ? recentTelemetry[0] : null
                    };

                    // Try to get typed combined sender from function context services
                    var provider = context.InstanceServices;
                    var combinedSender = provider?.GetService(typeof(IotRagFunctionApp.Services.ICombinedAlertsSender)) as IotRagFunctionApp.Services.ICombinedAlertsSender;
                    if (combinedSender != null)
                    {
                        var payload = System.Text.Json.JsonSerializer.Serialize(combined);
                        var msg = new Azure.Messaging.ServiceBus.ServiceBusMessage(payload) { MessageId = alert.Id };
                        await combinedSender.SendAsync(msg);
                        logger.LogInformation("Forwarded combined alert for {device} to combined-alerts-queue", alert.DeviceId);
                    }
                    else
                    {
                        logger.LogInformation("No combined-alerts sender configured; skipping forward.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error creating/forwarding combined alert: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing alert: {ex.Message}");
            }
        }
        */
    }
}
