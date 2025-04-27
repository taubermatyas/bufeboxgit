using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using BufeBackEnd.Controllers;
using BufeBackEnd.Models;
using System.Data.Entity;
using System;
using BufeBackEnd;

namespace BufeBackendTest
{
    [TestClass]
    public class KosarControllerTest
    {
        private Mock<BufeContext> mockContext;
        private Mock<DbSet<Kosar>> mockSet;
        private List<Kosar> data;
        private List<Vasarlo> vasarlok;
        private List<Dolgozo> dolgozok;
        private KosarController controller;

        [TestInitialize]
        public void Setup()
        {
            data = new List<Kosar>
            {
                new Kosar { Kkod = 1, Email = "a@b.com", Felnev = "admin", Idopont = DateTime.Now, Megjegyzes = "Megjegyzés", Elkeszult = false }
            };

            vasarlok = new List<Vasarlo> { new Vasarlo { Email = "a@b.com", Nev = "Teszt Vásárló" } };
            dolgozok = new List<Dolgozo> { new Dolgozo { Felnev = "admin", Nev = "Teszt Admin" } };

            var queryable = data.AsQueryable();
            mockSet = new Mock<DbSet<Kosar>>();
            mockSet.As<IQueryable<Kosar>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<Kosar>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Kosar>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Kosar>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(k => k.Kkod == (int)ids[0]));
            mockSet.Setup(m => m.Add(It.IsAny<Kosar>())).Callback<Kosar>(k => data.Add(k));
            mockSet.Setup(m => m.Remove(It.IsAny<Kosar>())).Callback<Kosar>(k => data.Remove(k));

            var mockVasarlok = new Mock<DbSet<Vasarlo>>();
            mockVasarlok.As<IQueryable<Vasarlo>>().Setup(m => m.Provider).Returns(vasarlok.AsQueryable().Provider);
            mockVasarlok.As<IQueryable<Vasarlo>>().Setup(m => m.Expression).Returns(vasarlok.AsQueryable().Expression);
            mockVasarlok.As<IQueryable<Vasarlo>>().Setup(m => m.ElementType).Returns(vasarlok.AsQueryable().ElementType);
            mockVasarlok.As<IQueryable<Vasarlo>>().Setup(m => m.GetEnumerator()).Returns(vasarlok.AsQueryable().GetEnumerator());

            var mockDolgozok = new Mock<DbSet<Dolgozo>>();
            mockDolgozok.As<IQueryable<Dolgozo>>().Setup(m => m.Provider).Returns(dolgozok.AsQueryable().Provider);
            mockDolgozok.As<IQueryable<Dolgozo>>().Setup(m => m.Expression).Returns(dolgozok.AsQueryable().Expression);
            mockDolgozok.As<IQueryable<Dolgozo>>().Setup(m => m.ElementType).Returns(dolgozok.AsQueryable().ElementType);
            mockDolgozok.As<IQueryable<Dolgozo>>().Setup(m => m.GetEnumerator()).Returns(dolgozok.AsQueryable().GetEnumerator());

            mockContext = new Mock<BufeContext>();
            mockContext.Setup(c => c.Kosarak).Returns(mockSet.Object);
            mockContext.Setup(c => c.Vasarlok).Returns(mockVasarlok.Object);
            mockContext.Setup(c => c.Dolgozok).Returns(mockDolgozok.Object);
            mockContext.Setup(c => c.SaveChanges()).Returns(1);

            controller = new KosarController();
            typeof(KosarController)
                .GetField("db", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(controller, mockContext.Object);
        }

        [TestMethod]
        public void GetKosarak_ReturnsAll()
        {
            var result = controller.GetKosarak() as OkNegotiatedContentResult<List<Kosar>>;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Content.Count);
        }

        [TestMethod]
        public void GetKosar_ValidId_ReturnsKosar()
        {
            var result = controller.GetKosar(1) as OkNegotiatedContentResult<Kosar>;
            Assert.IsNotNull(result);
            Assert.AreEqual("a@b.com", result.Content.Email);
        }

        [TestMethod]
        public void PostKosar_Valid_ReturnsCreated()
        {
            var uj = new Kosar { Kkod = 2, Email = "a@b.com", Felnev = "admin", Megjegyzes = "Új kosár" };
            var result = controller.PostKosar(uj);
            Assert.IsInstanceOfType(result, typeof(CreatedNegotiatedContentResult<Kosar>));
        }

        [TestMethod]
        public void PutKosar_Valid_ReturnsUpdated()
        {
            var frissit = new Kosar { Kkod = 1, Email = "a@b.com", Felnev = "admin", Megjegyzes = "Frissítve" };
            var result = controller.PutKosar(1, frissit) as OkNegotiatedContentResult<Kosar>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Frissítve", result.Content.Megjegyzes);
        }

        [TestMethod]
        public void UpdateKosarStatus_Valid_ReturnsOk()
        {
            var result = controller.UpdateKosarStatus(1, true) as OkNegotiatedContentResult<Kosar>;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Content.Elkeszult);
        }

        [TestMethod]
        public void DeleteKosar_Valid_ReturnsOk()
        {
            var result = controller.DeleteKosar(1);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void DeleteKosar_Invalid_ReturnsNotFound()
        {
            var result = controller.DeleteKosar(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
