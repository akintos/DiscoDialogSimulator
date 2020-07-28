using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator
{
    public static class Extensions
    {
        public static void SaveSetting(this System.Configuration.KeyValueConfigurationCollection col, string key, string value)
        {
            if (col[key] == null)
                col.Add(key, value);
            else
                col[key].Value = value;
        }
    }
}
