using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace IotRagFunctionApp.Functions
{
    public class CleanupOldTelemetry
    {
        // TEMPORARILY DISABLED - timer triggers not needed for HTTP endpoint testing
        public Task RunDisabled()
        {
            return Task.CompletedTask;
        }
    }
}
