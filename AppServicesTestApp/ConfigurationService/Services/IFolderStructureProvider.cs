using System;
using System.Collections.Generic;
using ConfigurationService.Data;

namespace ConfigurationService.Services
{
    public interface IFolderStructureProvider
    {
        FolderStructure CreateFolderStructure(string container, string folder, dynamic data = null, IReadOnlyDictionary<string, Func<dynamic, string>> accessors = null);
    }
}
