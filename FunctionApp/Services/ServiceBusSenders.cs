using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace IotRagFunctionApp.Services
{
    public interface IAlertsSender
    {
        Task SendAsync(ServiceBusMessage message);
    }

    public interface ICombinedAlertsSender
    {
        Task SendAsync(ServiceBusMessage message);
    }

    public class AlertsSender : IAlertsSender
    {
        private readonly ServiceBusSender _sender;
        public AlertsSender(ServiceBusSender sender) => _sender = sender;
        public Task SendAsync(ServiceBusMessage message) => _sender.SendMessageAsync(message);
    }

    public class CombinedAlertsSender : ICombinedAlertsSender
    {
        private readonly ServiceBusSender _sender;
        public CombinedAlertsSender(ServiceBusSender sender) => _sender = sender;
        public Task SendAsync(ServiceBusMessage message) => _sender.SendMessageAsync(message);
    }
}
