using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiscoDialogSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator.Tests
{
    [TestClass()]
    public class DialogueNumberTests
    {
        [TestMethod()]
        public void LoadJsonTest()
        {
            var dialogueNumber = DialogueNumber.LoadJson("../../../data/dialogue_no.json");
            Assert.AreEqual(dialogueNumber.Count, 65155);
        }
    }
}