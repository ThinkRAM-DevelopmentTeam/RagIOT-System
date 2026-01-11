using IotRagFunctionApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using System;
using System.Net.Http;
using System.Net;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        var cfg = context.Configuration;
        var cosmosConnection = cfg["CosmosConnection"] ?? "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqwm+DJgSepwBZSQdmkcJXcaXjRnxzrqlP7A==";

        // Create HttpClientHandler that ignores SSL cert validation (for local emulator only!)
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        var httpClient = new HttpClient(handler);

        // Register Cosmos DB client with custom HttpClient for SSL bypass
        var cosmosClient = new CosmosClient(cosmosConnection, new CosmosClientOptions 
        { 
            ConnectionMode = ConnectionMode.Direct,
            HttpClientFactory = () => httpClient
        });
        services.AddSingleton(cosmosClient);

        // Service Bus client and senders
        var serviceBusConnection = cfg["ServiceBusConnection"] ?? string.Empty;
        if (!string.IsNullOrEmpty(serviceBusConnection))
        {
            var sbClient = new ServiceBusClient(serviceBusConnection);
            services.AddSingleton(sbClient);
            // register typed wrappers so DI can resolve specific senders
            services.AddSingleton<IAlertsSender>(sp => new AlertsSender(sbClient.CreateSender("alerts-queue")));
            services.AddSingleton<ICombinedAlertsSender>(sp => new CombinedAlertsSender(sbClient.CreateSender("combined-alerts-queue")));
        }

        // Register services
        services.AddSingleton<ICosmosService, CosmosService>();
        services.AddSingleton<IEmbeddingService, MockEmbeddingService>();
        services.AddSingleton<IRagService, RagService>();

        // allow scoped HttpClient if needed
        services.AddHttpClient();

        // Initialize Cosmos on startup (with retry)
        var provider = services.BuildServiceProvider();
        var cosmosService = provider.GetRequiredService<ICosmosService>();
        
        int maxRetries = 3;
        int retryCount = 0;
        while (retryCount < maxRetries)
        {
            try
            {
                cosmosService.InitializeAsync().GetAwaiter().GetResult();
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    // Log but don't fail - allow functions to handle initialization errors
                    System.Console.WriteLine($"Cosmos initialization failed after {maxRetries} attempts: {ex.Message}");
                }
                else
                {
                    System.Threading.Thread.Sleep(1000 * retryCount); // exponential backoff
                }
            }
        }
    })
    .Build();

host.Run();
