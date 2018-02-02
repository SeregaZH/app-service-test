using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ConfigurationService.Data;
using Microsoft.Extensions.Configuration;

namespace ConfigurationService.Services
{
    public sealed class FolderStructureProvider: IFolderStructureProvider
    {
        private readonly IConfiguration _configuration;

        public FolderStructureProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FolderStructure CreateFolderStructure(string container, string folder, dynamic data = null, IReadOnlyDictionary<string, Func<dynamic, string>> accessors = null)
        {
            var section = _configuration.GetSection("Containers");
            var containerSections = section?.GetChildren();
            var containerRef = containerSections?.FirstOrDefault(x => x.Key.Equals(container));
            var folderRef = containerRef?.GetChildren()?.FirstOrDefault(x => x.Key.Equals(folder));
            return folderRef != null ? new FolderStructure(container, InterpolatePath(folderRef.Value, data, accessors)) : null;
        }

        private static string InterpolatePath<TData>(string rawPath, dynamic data, IReadOnlyDictionary<string, Func<TData, string>> accessors)
        {
            if (data != null && !string.IsNullOrEmpty(rawPath) && accessors != null)
            {
                return Regex.Replace(rawPath, @"{\w+}", match =>
                {
                    var matchValue = match.ToString().TrimStart('{').TrimEnd('}');
                    return accessors.ContainsKey(matchValue) ? accessors.GetValueOrDefault(matchValue)?.Invoke(data) : matchValue;
                });
            }

            return rawPath;
        }
    }
}
