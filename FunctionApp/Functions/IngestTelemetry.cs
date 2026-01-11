using System;
using System.Text.Json;
using Azure.Messaging.EventHubs;
using IotRagFunctionApp.Models;
using IotRagFunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IotRagFunctionApp.Functions
{
    public class IngestTelemetry
    {
        private readonly ICosmosService _cosmos;
        private readonly IAlertsSender? _alertsSender;

        public IngestTelemetry(ICosmosService cosmos, IAlertsSender? alertsSender = null)
        {
            _cosmos = cosmos;
            _alertsSender = alertsSender;
        }

        // TEMPORARILY DISABLED - Event Hubs emulator connection issues
        /*
        [Function("IngestTelemetry")]
        public async Task Run([EventHubTrigger("telemetry-hub", Connection = "EventHubsConnection")] EventData[] events, FunctionContext context)
        {
            var logger = context.GetLogger("IngestTelemetry");

            foreach (var eventData in events)
            {
                try
                {
                    string messageBody = System.Text.Encoding.UTF8.GetString(eventData.EventBody.ToArray());
                    var telemetry = JsonSerializer.Deserialize<DeviceTelemetry>(messageBody);

                    if (telemetry == null)
                    {
                        logger.LogWarning("Failed to deserialize telemetry message");
                        continue;
                    }

                    telemetry.IsAnomaly = telemetry.TemperatureC > 85 || telemetry.VibrationMm > 1.0;
                    await _cosmos.UpsertTelemetryAsync(telemetry);

                    logger.LogInformation("Ingested telemetry: {device} @ {time}, Temp={temp}°C, Vibration={vib}mm, Anomaly={anomaly}", telemetry.DeviceId, telemetry.Timestamp, telemetry.TemperatureC, telemetry.VibrationMm, telemetry.IsAnomaly);

                    if (telemetry.IsAnomaly)
                    {
                        var alert = new Alert { DeviceId = telemetry.DeviceId, Reason = "Anomaly detected: high temp/vibration", Timestamp = DateTime.UtcNow };

                        // Push alert to Service Bus queue if sender available
                        if (_alertsSender != null)
                        {
                            try
                            {
                                var payload = System.Text.Json.JsonSerializer.Serialize(alert);
                                var msg = new Azure.Messaging.ServiceBus.ServiceBusMessage(payload) { MessageId = alert.Id };
                                await _alertsSender.SendAsync(msg);
                                logger.LogWarning("ANOMALY DETECTED on {device}, alert queued {alertId}", telemetry.DeviceId, alert.Id);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError("Failed sending alert to Service Bus: {err}", ex.Message);
                                await _cosmos.UpsertAlertAsync(alert);
                                logger.LogWarning("ANOMALY DETECTED on {device}, alert persisted {alertId}", telemetry.DeviceId, alert.Id);
                            }
                        }
                        else
                        {
                            // No Service Bus configured, persist alert
                            await _cosmos.UpsertAlertAsync(alert);
                            logger.LogWarning("ANOMALY DETECTED on {device}, alert persisted {alertId}", telemetry.DeviceId, alert.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var logger2 = context.GetLogger("IngestTelemetry");
                    logger2.LogError($"Error processing event: {ex.Message}");
                }
            }
        }
        */
    }
}
