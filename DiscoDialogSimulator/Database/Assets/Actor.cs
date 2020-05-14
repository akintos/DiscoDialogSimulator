using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator.Database.Assets
{
    public class Actor : Asset
    {
        public string Name { get => LookupValue("Name"); }

        public override string ToString()
        {
            return Name;
        }
    }
}
