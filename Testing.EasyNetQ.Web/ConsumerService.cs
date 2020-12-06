using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Testing.EasyNetQ.Web.Configuration;

namespace Testing.EasyNetQ.Web
{
    public class ConsumerService : BackgroundService
    {
        private readonly IAdvancedBus _bus;
        private readonly IQueue _queue;

        public ConsumerService(IOptions<RabbitMqConfiguration> settings)
        {
            _bus = RabbitHutch.CreateBus(settings.Value.RabbitMqConnectionString).Advanced;
            _queue ??= _bus.QueueDeclare(settings.Value.Queue);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var exchange = _bus.ExchangeDeclare(typeof(SampleMessage).FullName, ExchangeType.Topic);

            _bus.Bind(exchange, _queue, "#");
            _bus.Consume<SampleMessage>(_queue, (message, info) =>
            {
                Console.WriteLine("Message consumed:");
                Console.WriteLine(message.Body.Text);
            });

            return Task.CompletedTask;
        }
    }
}
