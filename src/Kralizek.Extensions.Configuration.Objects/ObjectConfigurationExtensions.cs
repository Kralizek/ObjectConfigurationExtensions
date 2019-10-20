using System;
using Kralizek.Extensions.Configuration.Objects.Internal;
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.Configuration
{
    public static class ObjectConfigurationExtensions
    {
        public static IConfigurationBuilder AddObject<T>(this IConfigurationBuilder configurationBuilder, T objectToAdd, string rootSectionName = "")
        {
            if (objectToAdd == null)
            {
                return configurationBuilder;
            }

            if (rootSectionName == null)
            {
                throw new ArgumentNullException(nameof(rootSectionName));
            }

            var extractor = new JsonConfigurationExtractor();

            configurationBuilder.Add(new ObjectConfigurationSource(extractor, objectToAdd, rootSectionName));

            return configurationBuilder;
        }
    }
}
