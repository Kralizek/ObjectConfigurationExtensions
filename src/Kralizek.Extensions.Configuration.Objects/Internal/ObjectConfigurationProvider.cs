using System;
using Microsoft.Extensions.Configuration;

namespace Kralizek.Extensions.Configuration.Internal
{
    public class ObjectConfigurationProvider : ConfigurationProvider
    {
        private readonly string _rootSectionName;
        private readonly object _source;
        private readonly IConfigurationSerializer _serializer;

        public ObjectConfigurationProvider(IConfigurationSerializer serializer, object source, string rootSectionName)
        {
            _rootSectionName = rootSectionName ?? throw new ArgumentNullException(nameof(rootSectionName));
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public override void Load()
        {
            Data = _serializer.Serialize(_source, _rootSectionName);

            base.Load();
        }
    }
}