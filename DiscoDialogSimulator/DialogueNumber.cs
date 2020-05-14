using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator
{
    public class DialogueNumber : Dictionary<string, int>
    {
        public static DialogueNumber LoadJson(string path)
        {
            var serializer = new JsonSerializer();

            using (var reader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return serializer.Deserialize<DialogueNumber>(jsonReader);
            }
        } 

    }
}
