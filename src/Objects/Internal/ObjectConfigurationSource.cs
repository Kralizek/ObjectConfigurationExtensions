using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Kralizek.Extensions.Configuration.Internal;

internal sealed class ObjectConfigurationSource : IConfigurationSource
{
    private readonly Func<IDictionary<string, string?>> _dataFactory;

    public ObjectConfigurationSource(Func<IDictionary<string, string?>> dataFactory)
    {
        _dataFactory = dataFactory ?? throw new ArgumentNullException(nameof(dataFactory));
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new ObjectConfigurationProvider(_dataFactory);
    }
}
