using System;
using Microsoft.Extensions.Configuration;

namespace Kralizek.Extensions.Configuration.Objects.Internal
{
    public class ObjectConfigurationProvider : ConfigurationProvider
    {
        private readonly string _rootSectionName;
        private readonly object _source;
        private readonly IConfigurationExtractor _extractor;

        public ObjectConfigurationProvider(IConfigurationExtractor extractor, object source, string rootSectionName)
        {
            _rootSectionName = rootSectionName ?? throw new ArgumentNullException(nameof(rootSectionName));
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
        }

        public override void Load()
        {
            Data = _extractor.ExtractConfiguration(_source, _rootSectionName);

            base.Load();
        }
    }
}