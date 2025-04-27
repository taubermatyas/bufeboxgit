// KosarController.cs - javított változat
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using BufeBackEnd.Models;

namespace BufeBackEnd.Controllers
{
    [RoutePrefix("api/Kosar")]
    public class KosarController : ApiController
    {
        private BufeContext db = new BufeContext();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetKosarak()
        {
            try
            {
                var kosarak = db.Kosarak.ToList();
                return Ok(kosarak);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetKosar(int id)
        {
            try
            {
                var kosar = db.Kosarak.Find(id);
                if (kosar == null) return NotFound();
                return Ok(kosar);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostKosar([FromBody] Kosar kosar)
        {
            if (kosar == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            if (string.IsNullOrEmpty(kosar.Email) || string.IsNullOrEmpty(kosar.Felnev))
                return BadRequest("Email és Felhasználónév megadása kötelező.");

            try
            {
                var vasarlo = db.Vasarlok.FirstOrDefault(v => v.Email == kosar.Email);
                if (vasarlo == null)
                    return BadRequest($"A megadott email ({kosar.Email}) nem tartozik regisztrált vásárlóhoz.");

                var dolgozo = db.Dolgozok.FirstOrDefault(d => d.Felnev == kosar.Felnev);
                if (dolgozo == null)
                    return BadRequest($"A megadott felhasználónév ({kosar.Felnev}) nem létezik a Dolgozo táblában.");

                kosar.Idopont = kosar.Idopont == default ? DateTime.Now : kosar.Idopont;
                kosar.Elkeszult = false;

                db.Kosarak.Add(kosar);
                db.SaveChanges();

                return Created($"api/Kosar/{kosar.Kkod}", kosar);
            }
            catch (DbUpdateException dbEx)
            {
                return InternalServerError(dbEx);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("UpdateStatus/{id:int}")]
        public IHttpActionResult UpdateKosarStatus(int id, [FromBody] bool elkeszult)
        {
            try
            {
                var kosar = db.Kosarak.Find(id);
                if (kosar == null) return NotFound();

                kosar.Elkeszult = elkeszult;
                db.SaveChanges();
                return Ok(kosar);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult PutKosar(int id, [FromBody] Kosar kosar)
        {
            if (kosar == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            if (id != kosar.Kkod)
                return BadRequest("Az URL paraméter nem egyezik a kérés törzsével.");

            try
            {
                var existing = db.Kosarak.Find(id);
                if (existing == null) return NotFound();

                var vasarlo = db.Vasarlok.FirstOrDefault(v => v.Email == kosar.Email);
                if (vasarlo == null)
                    return BadRequest($"A megadott email ({kosar.Email}) nem tartozik regisztrált vásárlóhoz.");

                var dolgozo = db.Dolgozok.FirstOrDefault(d => d.Felnev == kosar.Felnev);
                if (dolgozo == null)
                    return BadRequest($"A megadott felhasználónév ({kosar.Felnev}) nem létezik a Dolgozo táblában.");

                existing.Idopont = kosar.Idopont;
                existing.Email = kosar.Email;
                existing.Felnev = kosar.Felnev;
                existing.Megjegyzes = kosar.Megjegyzes;

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
        public IHttpActionResult DeleteKosar(int id)
        {
            try
            {
                var kosar = db.Kosarak.Find(id);
                if (kosar == null) return NotFound();

                db.Kosarak.Remove(kosar);
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