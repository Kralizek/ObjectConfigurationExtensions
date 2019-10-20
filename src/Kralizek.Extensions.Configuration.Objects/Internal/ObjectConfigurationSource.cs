using System;
using Microsoft.Extensions.Configuration;

namespace Kralizek.Extensions.Configuration.Objects.Internal
{
    public class ObjectConfigurationSource : IConfigurationSource
    {
        private readonly string _rootSectionName;
        private readonly object _source;
        private readonly IConfigurationExtractor _extractor;

        public ObjectConfigurationSource(IConfigurationExtractor extractor, object source, string rootSectionName)
        {
            _rootSectionName = rootSectionName ?? throw new ArgumentNullException(nameof(rootSectionName));
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ObjectConfigurationProvider(_extractor, _source, _rootSectionName);
        }
    }
}