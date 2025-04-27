using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using BufeBackEnd.Controllers;
using BufeBackEnd.Models;
using System.Data.Entity;
using BufeBackEnd;

namespace BufeBackEndTest
{
    [TestClass]
    public class DolgozoControllerTest
    {
        private Mock<BufeContext> mockContext;
        private Mock<DbSet<Dolgozo>> mockSet;
        private List<Dolgozo> data;
        private DolgozoController controller;

        [TestInitialize]
        public void Setup()
        {
            data = new List<Dolgozo>
            {
                new Dolgozo { Felnev = "admin", Nev = "Admin Felhasználó", Jelszo = "titok" },
                new Dolgozo { Felnev = "user", Nev = "Normál Felhasználó", Jelszo = "1234" }
            };

            var queryable = data.AsQueryable();

            mockSet = new Mock<DbSet<Dolgozo>>();
            mockSet.As<IQueryable<Dolgozo>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<Dolgozo>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Dolgozo>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Dolgozo>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.Felnev == (string)ids[0]));
            mockSet.Setup(m => m.Add(It.IsAny<Dolgozo>())).Callback<Dolgozo>(d => data.Add(d));
            mockSet.Setup(m => m.Remove(It.IsAny<Dolgozo>())).Callback<Dolgozo>(d => data.Remove(d));

            mockContext = new Mock<BufeContext>();
            mockContext.Setup(c => c.Dolgozok).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChanges()).Returns(1);

            controller = new DolgozoController();
            typeof(DolgozoController)
                .GetField("db", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(controller, mockContext.Object);
        }

        [TestMethod]
        public void GetDolgozok_ReturnsAll()
        {
            var result = controller.GetDolgozok() as OkNegotiatedContentResult<List<Dolgozo>>;
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Content.Count);
        }

        [TestMethod]
        public void GetDolgozo_ValidFelnev_ReturnsCorrect()
        {
            var result = controller.GetDolgozo("admin") as OkNegotiatedContentResult<Dolgozo>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Admin Felhasználó", result.Content.Nev);
        }

        [TestMethod]
        public void GetDolgozo_InvalidFelnev_ReturnsNotFound()
        {
            var result = controller.GetDolgozo("nemletezo");
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PostDolgozo_New_ReturnsCreated()
        {
            var ujDolgozo = new Dolgozo { Felnev = "uj", Nev = "Új Dolgozó", Jelszo = "pwd" };
            var result = controller.PostDolgozo(ujDolgozo) as CreatedNegotiatedContentResult<Dolgozo>;
            Assert.IsNotNull(result);
            Assert.AreEqual("uj", result.Content.Felnev);
        }

        [TestMethod]
        public void PutDolgozo_Valid_ReturnsOk()
        {
            var frissitett = new Dolgozo { Felnev = "admin", Nev = "Frissítve", Jelszo = "ujjelszo" };
            var result = controller.PutDolgozo("admin", frissitett) as OkNegotiatedContentResult<Dolgozo>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Frissítve", result.Content.Nev);
        }

        [TestMethod]
        public void DeleteDolgozo_Valid_ReturnsOk()
        {
            var result = controller.DeleteDolgozo("user");
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void DeleteDolgozo_Invalid_ReturnsNotFound()
        {
            var result = controller.DeleteDolgozo("nincs");
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
