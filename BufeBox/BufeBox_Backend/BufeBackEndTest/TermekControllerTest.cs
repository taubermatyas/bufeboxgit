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
    public class TermekControllerTest
    {
        private Mock<BufeContext> mockContext;
        private Mock<DbSet<Termek>> mockTermekek;
        private Mock<DbSet<Kategoria>> mockKategoriak;

        private List<Termek> termekLista;
        private List<Kategoria> kategoriaLista;
        private TermekController controller;

        [TestInitialize]
        public void Setup()
        {
            kategoriaLista = new List<Kategoria>
            {
                new Kategoria { Kid = 1, Knev = "Italok" }
            };

            termekLista = new List<Termek>
            {
                new Termek
                {
                    Tid = 1,
                    Tnev = "Cola",
                    Mennyiseg = 10,
                    Kiszereles = "0.5L",
                    Ar = 300,
                    Afa = 27,
                    Kid = 1,
                    KepUrl = "cola.jpg"
                }
            };

            var qTermekek = termekLista.AsQueryable();
            var qKategoriak = kategoriaLista.AsQueryable();

            mockTermekek = new Mock<DbSet<Termek>>();
            mockTermekek.As<IQueryable<Termek>>().Setup(m => m.Provider).Returns(qTermekek.Provider);
            mockTermekek.As<IQueryable<Termek>>().Setup(m => m.Expression).Returns(qTermekek.Expression);
            mockTermekek.As<IQueryable<Termek>>().Setup(m => m.ElementType).Returns(qTermekek.ElementType);
            mockTermekek.As<IQueryable<Termek>>().Setup(m => m.GetEnumerator()).Returns(qTermekek.GetEnumerator());
            mockTermekek.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => termekLista.FirstOrDefault(t => t.Tid == (int)ids[0]));
            mockTermekek.Setup(m => m.Add(It.IsAny<Termek>())).Callback<Termek>(t => termekLista.Add(t));
            mockTermekek.Setup(m => m.Remove(It.IsAny<Termek>())).Callback<Termek>(t => termekLista.Remove(t));

            mockKategoriak = new Mock<DbSet<Kategoria>>();
            mockKategoriak.As<IQueryable<Kategoria>>().Setup(m => m.Provider).Returns(qKategoriak.Provider);
            mockKategoriak.As<IQueryable<Kategoria>>().Setup(m => m.Expression).Returns(qKategoriak.Expression);
            mockKategoriak.As<IQueryable<Kategoria>>().Setup(m => m.ElementType).Returns(qKategoriak.ElementType);
            mockKategoriak.As<IQueryable<Kategoria>>().Setup(m => m.GetEnumerator()).Returns(qKategoriak.GetEnumerator());

            mockContext = new Mock<BufeContext>();
            mockContext.Setup(c => c.Termekek).Returns(mockTermekek.Object);
            mockContext.Setup(c => c.Kategoriak).Returns(mockKategoriak.Object);
            mockContext.Setup(c => c.SaveChanges()).Returns(1);

            controller = new TermekController();
            typeof(TermekController)
                .GetField("db", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(controller, mockContext.Object);
        }


        [TestMethod]
        public void GetTermek_ValidId_ReturnsCorrect()
        {
            var result = controller.GetTermek(1) as OkNegotiatedContentResult<Termek>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Cola", result.Content.Tnev);
        }

        [TestMethod]
        public void PostTermek_Valid_ReturnsCreated()
        {
            var uj = new Termek
            {
                Tid = 2,
                Tnev = "Fanta",
                Mennyiseg = 15,
                Kiszereles = "0.5L",
                Ar = 350,
                Afa = 27,
                Kid = 1,
                KepUrl = "fanta.jpg"
            };

            var result = controller.PostTermek(uj);
            Assert.IsInstanceOfType(result, typeof(CreatedNegotiatedContentResult<Termek>));
        }

        [TestMethod]
        public void PutTermek_Valid_ReturnsUpdated()
        {
            var modositott = new Termek
            {
                Tid = 1,
                Tnev = "Pepsi",
                Mennyiseg = 12,
                Kiszereles = "0.5L",
                Ar = 320,
                Afa = 27,
                Kid = 1,
                KepUrl = "pepsi.jpg"
            };

            var result = controller.PutTermek(1, modositott) as OkNegotiatedContentResult<Termek>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Pepsi", result.Content.Tnev);
        }

        [TestMethod]
        public void DeleteTermek_Valid_ReturnsOk()
        {
            var result = controller.DeleteTermek(1);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void DeleteTermek_Invalid_ReturnsNotFound()
        {
            var result = controller.DeleteTermek(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
