using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiscoDialogSimulator.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator.Database.Tests
{
    [TestClass()]
    public class DialogueDatabaseTests
    {
        [TestMethod()]
        public void DeserializeTest()
        {
            var db = DialogueDatabase.Deserialize("../../../data/database.json");

            Console.WriteLine(db.actors.Count);
        }
    }
}