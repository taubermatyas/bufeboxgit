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
    public class KategoriaControllerTest
    {
        private Mock<BufeContext> mockContext;
        private Mock<DbSet<Kategoria>> mockSet;
        private List<Kategoria> data;
        private KategoriaController controller;

        [TestInitialize]
        public void Setup()
        {
            data = new List<Kategoria>
            {
                new Kategoria { Kid = 1, Knev = "Italok" },
                new Kategoria { Kid = 2, Knev = "Ételek" }
            };

            var queryable = data.AsQueryable();

            mockSet = new Mock<DbSet<Kategoria>>();
            mockSet.As<IQueryable<Kategoria>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<Kategoria>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Kategoria>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Kategoria>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(k => k.Kid == (int)ids[0]));
            mockSet.Setup(m => m.Add(It.IsAny<Kategoria>())).Callback<Kategoria>(k => data.Add(k));
            mockSet.Setup(m => m.Remove(It.IsAny<Kategoria>())).Callback<Kategoria>(k => data.Remove(k));

            mockContext = new Mock<BufeContext>();
            mockContext.Setup(c => c.Kategoriak).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChanges()).Returns(1);

            controller = new KategoriaController();
            typeof(KategoriaController)
                .GetField("db", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(controller, mockContext.Object);
        }

        [TestMethod]
        public void GetKategoriak_ReturnsAll()
        {
            var result = controller.GetKategoriak() as OkNegotiatedContentResult<List<Kategoria>>;
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Content.Count);
        }

        [TestMethod]
        public void GetKategoria_ValidId_ReturnsCorrect()
        {
            var result = controller.GetKategoria(1) as OkNegotiatedContentResult<Kategoria>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Italok", result.Content.Knev);
        }

        [TestMethod]
        public void GetKategoria_InvalidId_ReturnsNotFound()
        {
            var result = controller.GetKategoria(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PostKategoria_New_ReturnsCreated()
        {
            var uj = new Kategoria { Kid = 3, Knev = "Snack" };
            var result = controller.PostKategoria(uj) as CreatedNegotiatedContentResult<Kategoria>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Snack", result.Content.Knev);
        }

        [TestMethod]
        public void PutKategoria_Valid_ReturnsUpdated()
        {
            var frissitett = new Kategoria { Kid = 1, Knev = "Üdítők" };
            var result = controller.PutKategoria(1, frissitett) as OkNegotiatedContentResult<Kategoria>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Üdítők", result.Content.Knev);
        }

        [TestMethod]
        public void DeleteKategoria_Valid_ReturnsOk()
        {
            var result = controller.DeleteKategoria(2);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void DeleteKategoria_Invalid_ReturnsNotFound()
        {
            var result = controller.DeleteKategoria(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
