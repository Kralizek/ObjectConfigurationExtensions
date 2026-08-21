using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Kralizek.Extensions.Configuration.Internal;

internal sealed class ObjectConfigurationProvider : ConfigurationProvider
{
    private readonly Func<IDictionary<string, string?>> _dataFactory;

    public ObjectConfigurationProvider(Func<IDictionary<string, string?>> dataFactory)
    {
        _dataFactory = dataFactory ?? throw new ArgumentNullException(nameof(dataFactory));
    }

    public override void Load()
    {
        Data = _dataFactory();

        base.Load();
    }
}
