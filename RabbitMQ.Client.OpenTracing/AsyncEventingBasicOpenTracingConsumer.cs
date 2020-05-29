using System;
using System.Threading.Tasks;
using OpenTracing;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.OpenTracing
{
    public class AsyncEventingBasicOpenTracingConsumer : AsyncEventingBasicConsumer
    {
        private readonly ITracer _tracer;

        public AsyncEventingBasicOpenTracingConsumer(IModel model, ITracer tracer) : base(model)
        {
            _tracer = tracer;
        }

        public override async Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered,
            string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var spanBuilder =
                SpanBuilderHelper.BuildConsumerSpan(consumerTag, deliveryTag, exchange, routingKey, properties, _tracer);

            using (spanBuilder.StartActive(true))
            {
                await base.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties,
                    body).ConfigureAwait(false);
            }
        }
    }
}