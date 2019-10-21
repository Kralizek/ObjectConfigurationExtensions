using System;
using Kralizek.Extensions.Configuration.Internal;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.Configuration
{
    public static class ObjectConfigurationExtensions
    {
        public static IConfigurationBuilder AddObject(this IConfigurationBuilder configurationBuilder, object objectToAdd, string rootSectionName = "")
        {
            if (objectToAdd == null)
            {
                return configurationBuilder;
            }

            if (rootSectionName == null)
            {
                throw new ArgumentNullException(nameof(rootSectionName));
            }

            var serializer = new JsonConfigurationSerializer();

            configurationBuilder.Add(new ObjectConfigurationSource(serializer, objectToAdd, rootSectionName));

            return configurationBuilder;
        }
    }
}
