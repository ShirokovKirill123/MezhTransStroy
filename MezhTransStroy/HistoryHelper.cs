using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MezhTransStroy
{
    public static class HistoryHelper
    {
        private static string историяPath = "перемещения.json";

        public static void Add(string сообщение)
        {
            List<string> история = new List<string>();

            if (File.Exists(историяPath))
            {
                string json = File.ReadAllText(историяPath);
                история = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }

            if (!история.Contains(сообщение))
            {
                история.Add(сообщение);
                File.WriteAllText(историяPath, JsonConvert.SerializeObject(история, Formatting.Indented));
            }
        }

        public static List<string> Get()
        {
            if (File.Exists(историяPath))
            {
                string json = File.ReadAllText(историяPath);
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }

            return new List<string>();
        }
    }
}
