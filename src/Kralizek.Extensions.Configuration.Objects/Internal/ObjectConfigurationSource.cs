using System;
using Microsoft.Extensions.Configuration;

namespace Kralizek.Extensions.Configuration.Internal
{
    public class ObjectConfigurationSource : IConfigurationSource
    {
        private readonly string _rootSectionName;
        private readonly object _source;
        private readonly IConfigurationSerializer _serializer;

        public ObjectConfigurationSource(IConfigurationSerializer serializer, object source, string rootSectionName)
        {
            _rootSectionName = rootSectionName ?? throw new ArgumentNullException(nameof(rootSectionName));
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ObjectConfigurationProvider(_serializer, _source, _rootSectionName);
        }
    }
}