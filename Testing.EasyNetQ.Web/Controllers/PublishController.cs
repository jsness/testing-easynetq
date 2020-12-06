using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Testing.EasyNetQ.Web.Configuration;

namespace Testing.EasyNetQ.Web.Controllers
{
    public class PublishController : ControllerBase
    {
        private readonly RabbitMqConfiguration _settings;

        public PublishController(IOptions<RabbitMqConfiguration> settings) => _settings = settings.Value;

        [HttpGet]
        [Route("publish")]
        public IActionResult Publish(string text)
        {
            using var bus = RabbitHutch.CreateBus(_settings.RabbitMqConnectionString).Advanced;
            var exchange = bus.ExchangeDeclare(typeof(SampleMessage).FullName, ExchangeType.Topic);

            var message = new SampleMessage
            {
                Text = text
            };

            bus.Publish(exchange, "", true, new Message<SampleMessage>(message));

            return Ok();
        }
    }
}
