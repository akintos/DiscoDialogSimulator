using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiscoDialogSimulator.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using DiscoDialogSimulator.Database.Assets;

namespace DiscoDialogSimulator.Database.Tests
{
    [TestClass()]
    public class FieldsConverterTests
    {
        string sample = "{\"fields\":[{\"title\":\"Title\",\"value\":\"START\",\"type\":0,\"typeString\":\"\"},{\"title\":\"Articy Id\",\"value\":\"0x0100001700001F84\",\"type\":0,\"typeString\":\"\"},{\"title\":\"Sequence\",\"value\":\"Continue()\",\"type\":0,\"typeString\":\"\"}],\"fields2\":[{\"title\":\"Title\",\"value\":\"input\",\"type\":0,\"typeString\":\"\"},{\"title\":\"Sequence\",\"value\":\"Continue()\",\"type\":0,\"typeString\":\"\"},{\"title\":\"Articy Id\",\"value\":\"0x0100001700001F85\",\"type\":0,\"typeString\":\"\"},{\"title\":\"InputId\",\"value\":\"0x0100001700001F85\",\"type\":0,\"typeString\":\"\"}]}";

        class SampleClass
        {
            public Fields fields { get; set; }
            public Fields fields2 { get; set; }
        }

        [TestMethod()]
        public void ReadJsonTest()
        {
            var reader = new JsonTextReader(new StringReader(sample));

            var serializer = new JsonSerializer();
            serializer.Converters.Add(new FieldsConverter());

            var result = serializer.Deserialize<SampleClass>(reader);
        }
    }
}