using System.Collections.Generic;

namespace Kralizek.Extensions.Configuration;

public interface IConfigurationSerializer
{
    IDictionary<string, string?> Serialize(object source, string rootSectionName);
}