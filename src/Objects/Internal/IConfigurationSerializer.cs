using System.Collections.Generic;

namespace Kralizek.Extensions.Configuration.Internal;

public interface IConfigurationSerializer
{
    IDictionary<string, string?> Serialize(object source, string rootSectionName);
}