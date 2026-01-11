using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs.Producer;

public class DeviceTelemetry
{
    public string DeviceId { get; set; } = default!;
    public DateTime Timestamp { get; set; }
    public double TemperatureC { get; set; }
    public double VibrationMm { get; set; }
    public bool IsAnomaly { get; set; }
}

class Program
{
    private const string EventHubsConnectionString = "Endpoint=sb://localhost/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY;EntityPath=telemetry-hub";
    private const string EventHubName = "telemetry-hub";

    private static readonly string[] DeviceIds =
        { "pump-001", "pump-002", "pump-003", "compressor-001", "compressor-002" };

    private static readonly Random Random = new();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting telemetry generator. Press Ctrl+C to stop.");

        await using var producer = new EventHubProducerClient(EventHubsConnectionString, EventHubName);

        while (true)
        {
            try
            {
                using EventDataBatch batch = await producer.CreateBatchAsync();

                for (int i = 0; i < 10; i++)
                {
                    var telemetry = CreateRandomTelemetry();
                    string json = JsonSerializer.Serialize(telemetry);
                    var eventData = new Azure.Messaging.EventHubs.EventData(Encoding.UTF8.GetBytes(json));

                    if (!batch.TryAdd(eventData))
                    {
                        await producer.SendAsync(batch);
                        Console.WriteLine($"Batch sent, event dropped (batch full): {json}");
                        break;
                    }

                    Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Generated: {json}");
                }

                await producer.SendAsync(batch);
                Console.WriteLine($"Batch sent at {DateTime.UtcNow:O}");

                await Task.Delay(TimeSpan.FromSeconds(3));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }

    private static DeviceTelemetry CreateRandomTelemetry()
    {
        string deviceId = DeviceIds[Random.Next(DeviceIds.Length)];
        bool isPump = deviceId.StartsWith("pump");

        double baseTemp = isPump ? 70 : 60;
        double baseVibration = isPump ? 0.5 : 0.3;

        bool anomaly = Random.NextDouble() < 0.05;

        double temp = baseTemp + (Random.NextDouble() * 10 - 5);
        double vibration = Math.Max(baseVibration + (Random.NextDouble() * 0.4 - 0.2), 0);

        if (anomaly)
        {
            temp += 25;
            vibration += 0.8;
        }

        return new DeviceTelemetry
        {
            DeviceId = deviceId,
            Timestamp = DateTime.UtcNow,
            TemperatureC = Math.Round(temp, 2),
            VibrationMm = Math.Round(vibration, 3),
            IsAnomaly = anomaly
        };
    }
}
