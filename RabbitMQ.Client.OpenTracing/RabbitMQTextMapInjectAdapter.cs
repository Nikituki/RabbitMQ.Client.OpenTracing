using System;
using System.Collections;
using System.Collections.Generic;
using OpenTracing;
using OpenTracing.Propagation;

namespace RabbitMQ.Client.OpenTracing
{
    public class RabbitMqTextMapInjectAdapter : ITextMap
    {
        private readonly IDictionary<string, object> _context;
        
        public RabbitMqTextMapInjectAdapter(IDictionary<string, object> context)
        {
            _context = context;
        }
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new NotSupportedException(
                $"{nameof(TextMapInjectAdapter)} should only be used with {nameof(ITracer)}.{nameof(ITracer.Inject)}");
        }

        public void Set(string key, string value)
        {
            _context.Add(key, value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}