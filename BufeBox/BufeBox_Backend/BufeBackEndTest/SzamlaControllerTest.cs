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
    public class SzamlaControllerTest
    {
        private Mock<BufeContext> mockContext;
        private Mock<DbSet<Szamla>> mockSzamlaSet;
        private Mock<DbSet<Kosar>> mockKosarSet;
        private Mock<DbSet<Vasarlo>> mockVasarloSet;

        private List<Szamla> szamlak;
        private List<Kosar> kosarak;
        private List<Vasarlo> vasarlok;

        private SzamlaController controller;

        [TestInitialize]
        public void Setup()
        {
            szamlak = new List<Szamla>
            {
                new Szamla { Szkod = 1, Email = "teszt@pelda.hu", Osszeg = 2500 }
            };

            kosarak = new List<Kosar>
            {
                new Kosar { Kkod = 1, Email = "teszt@pelda.hu", Elkeszult = true }
            };

            vasarlok = new List<Vasarlo>
            {
                new Vasarlo { Email = "teszt@pelda.hu", Nev = "Teszt Vásárló" }
            };

            var qSzamlak = szamlak.AsQueryable();
            var qKosarak = kosarak.AsQueryable();
            var qVasarlok = vasarlok.AsQueryable();

            mockSzamlaSet = new Mock<DbSet<Szamla>>();
            mockSzamlaSet.As<IQueryable<Szamla>>().Setup(m => m.Provider).Returns(qSzamlak.Provider);
            mockSzamlaSet.As<IQueryable<Szamla>>().Setup(m => m.Expression).Returns(qSzamlak.Expression);
            mockSzamlaSet.As<IQueryable<Szamla>>().Setup(m => m.ElementType).Returns(qSzamlak.ElementType);
            mockSzamlaSet.As<IQueryable<Szamla>>().Setup(m => m.GetEnumerator()).Returns(qSzamlak.GetEnumerator());
            mockSzamlaSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => szamlak.FirstOrDefault(s => s.Szkod == (int)ids[0]));
            mockSzamlaSet.Setup(m => m.Add(It.IsAny<Szamla>())).Callback<Szamla>(s => szamlak.Add(s));
            mockSzamlaSet.Setup(m => m.Remove(It.IsAny<Szamla>())).Callback<Szamla>(s => szamlak.Remove(s));

            mockKosarSet = new Mock<DbSet<Kosar>>();
            mockKosarSet.As<IQueryable<Kosar>>().Setup(m => m.Provider).Returns(qKosarak.Provider);
            mockKosarSet.As<IQueryable<Kosar>>().Setup(m => m.Expression).Returns(qKosarak.Expression);
            mockKosarSet.As<IQueryable<Kosar>>().Setup(m => m.ElementType).Returns(qKosarak.ElementType);
            mockKosarSet.As<IQueryable<Kosar>>().Setup(m => m.GetEnumerator()).Returns(qKosarak.GetEnumerator());
            mockKosarSet.Setup(m => m.Remove(It.IsAny<Kosar>())).Callback<Kosar>(k => kosarak.Remove(k));

            mockVasarloSet = new Mock<DbSet<Vasarlo>>();
            mockVasarloSet.As<IQueryable<Vasarlo>>().Setup(m => m.Provider).Returns(qVasarlok.Provider);
            mockVasarloSet.As<IQueryable<Vasarlo>>().Setup(m => m.Expression).Returns(qVasarlok.Expression);
            mockVasarloSet.As<IQueryable<Vasarlo>>().Setup(m => m.ElementType).Returns(qVasarlok.ElementType);
            mockVasarloSet.As<IQueryable<Vasarlo>>().Setup(m => m.GetEnumerator()).Returns(qVasarlok.GetEnumerator());

            mockContext = new Mock<BufeContext>();
            mockContext.Setup(c => c.Szamlak).Returns(mockSzamlaSet.Object);
            mockContext.Setup(c => c.Kosarak).Returns(mockKosarSet.Object);
            mockContext.Setup(c => c.Vasarlok).Returns(mockVasarloSet.Object);
            mockContext.Setup(c => c.SaveChanges()).Returns(1);

            controller = new SzamlaController();
            typeof(SzamlaController)
                .GetField("db", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(controller, mockContext.Object);
        }

        [TestMethod]
        public void GetSzamlak_ReturnsAll()
        {
            var result = controller.GetSzamlak() as OkNegotiatedContentResult<List<Szamla>>;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Content.Count);
        }

        [TestMethod]
        public void GetSzamla_ValidId_ReturnsCorrect()
        {
            var result = controller.GetSzamla(1) as OkNegotiatedContentResult<Szamla>;
            Assert.IsNotNull(result);
            Assert.AreEqual("teszt@pelda.hu", result.Content.Email);
        }

        [TestMethod]
        public void PostSzamla_Valid_ReturnsCreated()
        {
            var uj = new Szamla { Szkod = 2, Email = "teszt@pelda.hu", Osszeg = 3000 };
            var result = controller.PostSzamla(uj);
            Assert.IsInstanceOfType(result, typeof(CreatedNegotiatedContentResult<Szamla>));
        }

        [TestMethod]
        public void PutSzamla_Valid_ReturnsUpdated()
        {
            var frissitett = new Szamla { Szkod = 1, Email = "teszt@pelda.hu", Osszeg = 4000 };
            var result = controller.PutSzamla(1, frissitett) as OkNegotiatedContentResult<Szamla>;
            Assert.IsNotNull(result);
            Assert.AreEqual(4000, result.Content.Osszeg);
        }

        [TestMethod]
        public void DeleteSzamla_Valid_ReturnsOk()
        {
            var result = controller.DeleteSzamla(1);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void DeleteSzamla_Invalid_ReturnsNotFound()
        {
            var result = controller.DeleteSzamla(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
