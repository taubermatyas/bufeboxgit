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
    public class VasarloControllerTest
    {
        private Mock<BufeContext> mockContext;
        private Mock<DbSet<Vasarlo>> mockSet;
        private List<Vasarlo> vasarloLista;
        private VasarloController controller;

        [TestInitialize]
        public void Setup()
        {
            vasarloLista = new List<Vasarlo>
            {
                new Vasarlo { Email = "teszt@pelda.hu", Nev = "Teszt Elek", Jelszo = "Titkos123!" }
            };

            var queryable = vasarloLista.AsQueryable();
            mockSet = new Mock<DbSet<Vasarlo>>();
            mockSet.As<IQueryable<Vasarlo>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<Vasarlo>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Vasarlo>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Vasarlo>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => vasarloLista.FirstOrDefault(v => v.Email == (string)ids[0]));
            mockSet.Setup(m => m.Add(It.IsAny<Vasarlo>())).Callback<Vasarlo>(v => vasarloLista.Add(v));
            mockSet.Setup(m => m.Remove(It.IsAny<Vasarlo>())).Callback<Vasarlo>(v => vasarloLista.Remove(v));

            mockContext = new Mock<BufeContext>();
            mockContext.Setup(c => c.Vasarlok).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChanges()).Returns(1);

            controller = new VasarloController();
            typeof(VasarloController)
                .GetField("db", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(controller, mockContext.Object);
        }

        [TestMethod]
        public void GetVasarlok_ReturnsAll()
        {
            var result = controller.GetVasarlok() as OkNegotiatedContentResult<List<Vasarlo>>;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Content.Count);
        }

        [TestMethod]
        public void GetVasarlo_ValidEmail_ReturnsCorrect()
        {
            var result = controller.GetVasarlo("teszt@pelda.hu") as OkNegotiatedContentResult<Vasarlo>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Teszt Elek", result.Content.Nev);
        }

        [TestMethod]
        public void PostVasarlo_Valid_ReturnsCreated()
        {
            var uj = new Vasarlo { Email = "uj@pelda.hu", Nev = "Új Teszt", Jelszo = "Jelszo123!" };
            var result = controller.PostVasarlo(uj);
            Assert.IsInstanceOfType(result, typeof(CreatedNegotiatedContentResult<Vasarlo>));
        }

        [TestMethod]
        public void PutVasarlo_Valid_ReturnsUpdated()
        {
            var frissit = new Vasarlo { Email = "teszt@pelda.hu", Nev = "Módosítva", Jelszo = "Titok1234!" };
            var result = controller.PutVasarlo("teszt@pelda.hu", frissit) as OkNegotiatedContentResult<Vasarlo>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Módosítva", result.Content.Nev);
        }

        [TestMethod]
        public void DeleteVasarlo_Valid_ReturnsOk()
        {
            var result = controller.DeleteVasarlo("teszt@pelda.hu");
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }


        [TestMethod]
        public void UpdateVasarlo_Valid_ReturnsUpdated()
        {
            var update = new Vasarlo { Email = "teszt@pelda.hu", Nev = "Friss", Jelszo = "Ujjelszo123!" };
            var result = controller.UpdateVasarlo(update) as OkNegotiatedContentResult<Vasarlo>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Friss", result.Content.Nev);
        }
    }
}
