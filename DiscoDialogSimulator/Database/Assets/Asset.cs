using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator.Database.Assets
{
    public class Asset
    {
        public int id { get; set; }

        public Fields fields { get; set; } = new Fields();

        public string this[string key] { get => fields[key]; set => fields[key] = value; }

        public string LookupValue(string fieldname)
        {
            return fields.TryGetValue(fieldname, out string value) ? value : null;
        }

        public bool TryGetValue(string key, out string value) => fields.TryGetValue(key, out value);

        public bool ContainsKey(string key) => fields.ContainsKey(key);

    }

    public class Fields : Dictionary<string, string>
    {

    }
}
