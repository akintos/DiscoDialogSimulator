using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator
{
    public class WeblateData : Dictionary<string, DialogueUnit>
    {
        public static WeblateData LoadJson(string path)
        {
            var serializer = new JsonSerializer();

            using (var reader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return serializer.Deserialize<WeblateData>(jsonReader);
            }
        }
    }

    public class DialogueUnit
    {
        [JsonProperty]
        public int id { get; set; }

        [JsonProperty]
        public int position { get; set; }
    }
}
