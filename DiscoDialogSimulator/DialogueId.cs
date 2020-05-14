using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator
{
    public class DialogueId : Dictionary<string, int>
    {
        public static DialogueId LoadJson(string path)
        {
            var serializer = new JsonSerializer();

            using (var reader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return serializer.Deserialize<DialogueId>(jsonReader);
            }
        }

    }
}
