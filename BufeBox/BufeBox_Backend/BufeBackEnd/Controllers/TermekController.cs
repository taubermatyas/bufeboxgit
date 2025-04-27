using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using BufeBackEnd.Models;

namespace BufeBackEnd.Controllers
{
    [RoutePrefix("api/Termek")]
    public class TermekController : ApiController
    {
        private BufeContext db = new BufeContext();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetTermekek()
        {
            try
            {
                var termekek = db.Termekek.Include(x => x.Kategoria).ToList();
                return Ok(termekek);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetTermek(int id)
        {
            try
            {
                var termek = db.Termekek.Find(id);
                if (termek == null) return NotFound();
                return Ok(termek);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostTermek([FromBody] Termek termek)
        {
            if (termek == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            try
            {
                if (db.Termekek.Any(t => t.Tnev == termek.Tnev && t.Kid == termek.Kid))
                    return BadRequest("Ez a termék már létezik ebben a kategóriában.");

                if (!db.Kategoriak.Any(k => k.Kid == termek.Kid))
                    return BadRequest("A megadott kategória nem létezik.");

                db.Termekek.Add(termek);
                db.SaveChanges();
                return Created($"api/Termek/{termek.Tid}", termek);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult PutTermek(int id, [FromBody] Termek termek)
        {
            if (termek == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            if (id != termek.Tid)
                return BadRequest("Az URL paraméter nem egyezik a kérés törzsével.");

            try
            {
                var existing = db.Termekek.Find(id);
                if (existing == null) return NotFound();

                existing.Tnev = termek.Tnev;
                existing.Mennyiseg = termek.Mennyiseg;
                existing.Kiszereles = termek.Kiszereles;
                existing.Ar = termek.Ar;
                existing.Afa = termek.Afa;
                existing.Kid = termek.Kid;
                existing.KepUrl = termek.KepUrl; // Új mező frissítése

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
        public IHttpActionResult DeleteTermek(int id)
        {
            try
            {
                var termek = db.Termekek.Find(id);
                if (termek == null) return NotFound();

                db.Termekek.Remove(termek);
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