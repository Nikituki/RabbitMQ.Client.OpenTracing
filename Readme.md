# RabbitMQ.Client.OpenTracing
Used to Inject and Extract [OpenTracing](https://opentracing.io/docs/overview/) SpanContext to RabbitMQ messages

Usage is as follows:

```c#
//for publishing
channel.BasicPublish("excchange", "routingKey",  tracer, body: body); //where channel is RabbitMQ.Client.IModel and tracer is OpenTracing.ITracer

//for consuming
var consumer = new EventingBasicOpenTracingConsumer(channel, tracer); //where channel is RabbitMQ.Client.IModel and tracer is OpenTracing.ITracer
```