using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using BufeBackEnd.Models;

namespace BufeBackEnd.Controllers
{
    [RoutePrefix("api/Szamla")]
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class SzamlaController : ApiController
    {
        private BufeContext db = new BufeContext();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetSzamlak()
        {
            try
            {
                var szamlak = db.Szamlak.ToList();
                return Ok(szamlak);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetSzamla(int id)
        {
            try
            {
                var szamla = db.Szamlak.Find(id);
                if (szamla == null) return NotFound();
                return Ok(szamla);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostSzamla([FromBody] Szamla szamla)
        {
            if (szamla == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            try
            {
                var vasarlo = db.Vasarlok.FirstOrDefault(v => v.Email == szamla.Email);
                if (vasarlo == null)
                    return BadRequest("A megadott emailhez nem tartozik vásárló.");

                var kosar = db.Kosarak.FirstOrDefault(k => k.Email == szamla.Email && k.Elkeszult);
                if (kosar == null)
                    return BadRequest("Nincs elkészült kosár ehhez az emailhez.");

                db.Szamlak.Add(szamla);
                db.SaveChanges();

                db.Kosarak.Remove(kosar);
                db.SaveChanges();

                return Created($"api/Szamla/{szamla.Szkod}", szamla);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult PutSzamla(int id, [FromBody] Szamla szamla)
        {
            if (szamla == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            if (id != szamla.Szkod)
                return BadRequest("Az URL paraméter nem egyezik a kérés törzsével.");

            try
            {
                var existing = db.Szamlak.Find(id);
                if (existing == null) return NotFound();

                existing.Osszeg = szamla.Osszeg;
                existing.Email = szamla.Email;

                db.SaveChanges();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteSzamla(int id)
        {
            try
            {
                var szamla = db.Szamlak.Find(id);
                if (szamla == null) return NotFound();

                db.Szamlak.Remove(szamla);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}