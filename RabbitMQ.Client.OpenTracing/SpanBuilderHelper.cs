using System;
using System.Linq;
using System.Text;
using OpenTracing;
using OpenTracing.Propagation;

namespace RabbitMQ.Client.OpenTracing
{
    public static class SpanBuilderHelper
    {
        public static ISpanBuilder BuildConsumerSpan(string consumerTag, ulong deliveryTag, string exchange, string routingKey,
            IBasicProperties properties, ITracer tracer)
        {
            var headers = properties.Headers.ToDictionary(x => x.Key,
                x => Encoding.UTF8.GetString(x.Value as byte[] ?? Array.Empty<byte>()));

            var parentSpanCtx = tracer.Extract(BuiltinFormats.TextMap,
                new TextMapExtractAdapter(headers));

            var spanBuilder = tracer.BuildSpan($"Consuming Message: exchange: {exchange}, routingKey: {routingKey}");

            if (parentSpanCtx != null)
            {
                spanBuilder = spanBuilder.AsChildOf(parentSpanCtx);
            }

            spanBuilder
                .WithTag("consumer-tag", consumerTag)
                .WithTag("delivery-tag", deliveryTag)
                .WithTag("exchange", exchange)
                .WithTag("routing-key", routingKey)
                .WithTag("message-id", properties.MessageId);
            return spanBuilder;
        }
        
        public static ISpanBuilder BuildPublisherSpan(string exchange, string routingKey, IBasicProperties basicProperties,
            ITracer tracer)
        {
            var operationName = $"Publishing Message: exchange: {exchange}, routingKey: {routingKey}";

            var spanBuilder = tracer.BuildSpan(operationName)
                .AsChildOf(tracer.ActiveSpan.Context)
                .WithTag("message-id", basicProperties.MessageId)
                .WithTag("app-id", basicProperties.AppId)
                .WithTag("exchange", exchange)
                .WithTag("routing-key", routingKey);
            return spanBuilder;
        }
    }
}