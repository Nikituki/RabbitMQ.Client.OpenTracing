using System;
using OpenTracing;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.OpenTracing
{
    public class EventingBasicOpenTracingConsumer : EventingBasicConsumer
    {
        private readonly ITracer _tracer;
        
        public EventingBasicOpenTracingConsumer(IModel model, ITracer tracer) : base(model)
        {
            _tracer = tracer;
        }
        
        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var spanBuilder = SpanBuilderHelper.BuildConsumerSpan(consumerTag, deliveryTag, exchange, routingKey, properties, _tracer);

            using (spanBuilder.StartActive(true))
            {
                base.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);
            }
        }
    }
}