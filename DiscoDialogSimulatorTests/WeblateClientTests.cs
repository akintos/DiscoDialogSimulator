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
    public class WeblateClientTests
    {
        [TestMethod()]
        public void WeblateClientTest()
        {
            var wlc = new WeblateClient("http://akintos.iptime.org/api/", "6nv14I3y4tIpE94JU0eoAYQ1NwW73becYmlnJ66C");
        }

        [TestMethod()]
        public void GetTranslationTest()
        {
            var wlc = new WeblateClient("http://akintos.iptime.org/api/", "6nv14I3y4tIpE94JU0eoAYQ1NwW73becYmlnJ66C");
            var tr = wlc.GetTranslation(724016);
            Assert.AreEqual(722, tr.position);
        }
    }
}