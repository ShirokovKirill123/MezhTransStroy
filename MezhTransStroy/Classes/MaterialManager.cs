using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Newtonsoft.Json;

namespace MezhTransStroy
{
    static class MaterialManager
    {
        private static readonly string filePath = "excludedMaterials.json";

        public static List<string> excludedMaterialsList { get; private set; } = new List<string>();

        public static void LoadExcludedMaterials()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                excludedMaterialsList = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }
        }

        public static void SaveExcludedMaterials()
        {
            var json = JsonConvert.SerializeObject(excludedMaterialsList);
            File.WriteAllText(filePath, json);
        }   
    }
}
