// KategoriaController.cs - javított változat
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using BufeBackEnd.Models;

namespace BufeBackEnd.Controllers
{
    [RoutePrefix("api/Kategoria")]
    public class KategoriaController : ApiController
    {
        private BufeContext db = new BufeContext();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetKategoriak()
        {
            try
            {
                var kategoriak = db.Kategoriak.ToList();
                return Ok(kategoriak);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetKategoria(int id)
        {
            try
            {
                var kategoria = db.Kategoriak.Find(id);
                if (kategoria == null) return NotFound();
                return Ok(kategoria);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostKategoria([FromBody] Kategoria kategoria)
        {
            if (kategoria == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            try
            {
                if (db.Kategoriak.Any(k => k.Knev == kategoria.Knev))
                    return BadRequest("Ez a kategórianév már létezik.");

                kategoria.Termekek = null;
                db.Kategoriak.Add(kategoria);
                db.SaveChanges();
                return Created($"api/Kategoria/{kategoria.Kid}", kategoria);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult PutKategoria(int id, [FromBody] Kategoria kategoria)
        {
            if (kategoria == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            if (id != kategoria.Kid)
                return BadRequest("Az URL paraméter nem egyezik a kérés törzsével.");

            try
            {
                var existing = db.Kategoriak.Find(id);
                if (existing == null) return NotFound();

                existing.Knev = kategoria.Knev;
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
        public IHttpActionResult DeleteKategoria(int id)
        {
            try
            {
                var kategoria = db.Kategoriak.Find(id);
                if (kategoria == null) return NotFound();

                db.Kategoriak.Remove(kategoria);
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

