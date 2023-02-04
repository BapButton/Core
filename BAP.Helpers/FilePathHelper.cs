
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace BAP.Helpers
{
    public static class FilePathHelper
    {
        public static IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        public static string GetFullPath<T>(string fileName)
        {
            string assemblyName = typeof(T).Assembly.FullName ?? "";
            assemblyName = assemblyName.Split(",")[0];
            string cacheKey = $"{assemblyName}_{fileName.ToLower()}";
            if (cache.TryGetValue(cacheKey, out string path))
            {
                return path;
            }

            string basePath = typeof(T).Assembly.Location;
            basePath = Path.GetDirectoryName(basePath) ?? "";
            if (String.IsNullOrEmpty(basePath))
            {
                return "";
            }
            //Find the file
            var files = Directory.GetFiles(basePath, fileName, SearchOption.AllDirectories);
            string fullPath = string.Empty;
            if (files.Length > 0)
            {
                fullPath = files.First();
            }
            var expiration = DateTime.Now.AddMinutes(60);
            if (string.IsNullOrEmpty(fullPath))
            {
                expiration = DateTime.Now.AddMinutes(5);
            }
            cache.Set(cacheKey, fullPath, absoluteExpiration: expiration);
            return fullPath;
        }
    }
}
