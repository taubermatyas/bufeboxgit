using Microsoft.VisualStudio.TestTools.UnitTesting;
using projekt;

namespace Bufetest
{
    [TestClass]
    public class LoginServiceTests
    {
        public string testKapcsolat = "server=localhost:3306,;uid=root;password=;database=bufe;ssl mode=none";

        [TestMethod]
        public void Belep_HelyesAdatokkal_SikeresBelepes()
        {
            var service = new LoginService(testKapcsolat);

            string nev = service.Belep("dolgozo1", "1234");
            Assert.AreEqual(nev, "Dolgozo1");
        }

        [TestMethod]
        public void Belep_HibasAdatokkal_SikertelenBelepes()
        {
            var service = new LoginService(testKapcsolat);

            string nev = service.Belep("rossz", "hibas");

            Assert.AreEqual(nev, "");
        }
    }
}
