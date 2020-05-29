using System;
using System.Collections.Generic;
using OpenTracing;
using OpenTracing.Propagation;

namespace RabbitMQ.Client.OpenTracing
{
    public static class IModelExensions
    {
        /// <summary>
        /// (Extension method) Convenience overload of BasicPublish.
        /// </summary>
        /// <remarks>
        /// The publication occurs with mandatory=false and immediate=false.
        /// </remarks>
        public static void BasicPublish(this IModel model, PublicationAddress addr, IBasicProperties basicProperties, ReadOnlyMemory<byte> body, ITracer tracer)
        {
            model.BasicPublish(addr.ExchangeName, addr.RoutingKey, basicProperties: basicProperties, body: body, tracer: tracer);
        }

        /// <summary>
        /// (Extension method) Convenience overload of BasicPublish.
        /// </summary>
        /// <remarks>
        /// The publication occurs with mandatory=false
        /// </remarks>
        public static void BasicPublish(this IModel model, string exchange, string routingKey, IBasicProperties basicProperties, ReadOnlyMemory<byte> body, ITracer tracer)
        {
            var spanBuilder = SpanBuilderHelper.BuildPublisherSpan(exchange, routingKey, basicProperties, tracer);

            using (spanBuilder.StartActive())
            {
                tracer.Inject(
                    tracer.ActiveSpan.Context,
                    BuiltinFormats.TextMap,
                    new RabbitMqTextMapInjectAdapter(basicProperties.Headers));

                model.BasicPublish(exchange, routingKey, false, basicProperties, body);
            }
        }

        /// <summary>
        /// (Spec method) Convenience overload of BasicPublish.
        /// </summary>
        public static void BasicPublish(this IModel model, string exchange, string routingKey, ITracer tracer, bool mandatory = false, IBasicProperties basicProperties = null, ReadOnlyMemory<byte> body = default)
        {
            var props = model.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>();
            
            var spanBuilder = SpanBuilderHelper.BuildPublisherSpan(exchange, routingKey, props, tracer);

            using (spanBuilder.StartActive())
            {
                tracer.Inject(
                    tracer.ActiveSpan.Context,
                    BuiltinFormats.TextMap,
                    new RabbitMqTextMapInjectAdapter(props.Headers));

                model.BasicPublish(exchange, routingKey, mandatory, props, body);
            }
        }
    }
}