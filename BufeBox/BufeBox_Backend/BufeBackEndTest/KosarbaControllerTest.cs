using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using BufeBackEnd.Controllers;
using BufeBackEnd.Models;
using System.Data.Entity;
using BufeBackEnd;

namespace BufeBackendTest
{
    [TestClass]
    public class KosarbaControllerTest
    {
        private Mock<BufeContext> mockContext;
        private Mock<DbSet<Kosarba>> mockSet;
        private List<Kosarba> kosarbaData;
        private List<Kosar> kosarak;
        private List<Termek> termekek;
        private KosarbaController controller;

        [TestInitialize]
        public void Setup()
        {
            kosarbaData = new List<Kosarba>
            {
                new Kosarba { Kkod = 1, Tid = 101, Tmenny = 1 }
            };

            kosarak = new List<Kosar>
            {
                new Kosar { Kkod = 1, Email = "a@b.com", Felnev = "dolgozo" }
            };

            termekek = new List<Termek>
            {
                new Termek { Tid = 101, Tnev = "Szendvics", Mennyiseg = 10 }
            };

            var qKosarba = kosarbaData.AsQueryable();
            var qKosarak = kosarak.AsQueryable();
            var qTermekek = termekek.AsQueryable();

            var mockKosarbaSet = new Mock<DbSet<Kosarba>>();
            mockKosarbaSet.As<IQueryable<Kosarba>>().Setup(m => m.Provider).Returns(qKosarba.Provider);
            mockKosarbaSet.As<IQueryable<Kosarba>>().Setup(m => m.Expression).Returns(qKosarba.Expression);
            mockKosarbaSet.As<IQueryable<Kosarba>>().Setup(m => m.ElementType).Returns(qKosarba.ElementType);
            mockKosarbaSet.As<IQueryable<Kosarba>>().Setup(m => m.GetEnumerator()).Returns(qKosarba.GetEnumerator());
            mockKosarbaSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids =>
                kosarbaData.FirstOrDefault(k => k.Kkod == (int)ids[0] && k.Tid == (int)ids[1]));
            mockKosarbaSet.Setup(m => m.Add(It.IsAny<Kosarba>())).Callback<Kosarba>(k => kosarbaData.Add(k));
            mockKosarbaSet.Setup(m => m.Remove(It.IsAny<Kosarba>())).Callback<Kosarba>(k => kosarbaData.Remove(k));

            var mockKosarak = new Mock<DbSet<Kosar>>();
            mockKosarak.As<IQueryable<Kosar>>().Setup(m => m.Provider).Returns(qKosarak.Provider);
            mockKosarak.As<IQueryable<Kosar>>().Setup(m => m.Expression).Returns(qKosarak.Expression);
            mockKosarak.As<IQueryable<Kosar>>().Setup(m => m.ElementType).Returns(qKosarak.ElementType);
            mockKosarak.As<IQueryable<Kosar>>().Setup(m => m.GetEnumerator()).Returns(qKosarak.GetEnumerator());

            var mockTermekek = new Mock<DbSet<Termek>>();
            mockTermekek.As<IQueryable<Termek>>().Setup(m => m.Provider).Returns(qTermekek.Provider);
            mockTermekek.As<IQueryable<Termek>>().Setup(m => m.Expression).Returns(qTermekek.Expression);
            mockTermekek.As<IQueryable<Termek>>().Setup(m => m.ElementType).Returns(qTermekek.ElementType);
            mockTermekek.As<IQueryable<Termek>>().Setup(m => m.GetEnumerator()).Returns(qTermekek.GetEnumerator());

            mockContext = new Mock<BufeContext>();
            mockContext.Setup(c => c.KosarbaTetelek).Returns(mockKosarbaSet.Object);
            mockContext.Setup(c => c.Kosarak).Returns(mockKosarak.Object);
            mockContext.Setup(c => c.Termekek).Returns(mockTermekek.Object);
            mockContext.Setup(c => c.SaveChanges()).Returns(1);

            controller = new KosarbaController();
            typeof(KosarbaController)
                .GetField("db", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(controller, mockContext.Object);
        }

        [TestMethod]
        public void GetKosarbaTetelek_ReturnsAll()
        {
            var result = controller.GetKosarbaTetelek() as OkNegotiatedContentResult<List<Kosarba>>;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Content.Count);
        }

        [TestMethod]
        public void GetKosarba_Valid_ReturnsTétel()
        {
            var result = controller.GetKosarba(1, 101) as OkNegotiatedContentResult<Kosarba>;
            Assert.IsNotNull(result);
            Assert.AreEqual(101, result.Content.Tid);
        }


        [TestMethod]
        public void PutKosarba_Valid_ReturnsOk()
        {
            var frissit = new Kosarba { Kkod = 1, Tid = 101, Tmenny = 2 };
            var result = controller.PutKosarba(1, 101, frissit);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Kosarba>));
        }

        [TestMethod]
        public void DeleteKosarba_Valid_ReturnsOk()
        {
            var result = controller.DeleteKosarba(1, 101);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
    }
}
