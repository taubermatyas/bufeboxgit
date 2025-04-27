// DolgozoController.cs - javított változat
using System;
using System.Linq;
using System.Web.Http;
using BufeBackEnd.Models;
using System.Web.Http.Cors;

namespace BufeBackEnd.Controllers
{
    [RoutePrefix("api/Dolgozo")]
    public class DolgozoController : ApiController
    {
        private BufeContext db = new BufeContext();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetDolgozok()
        {
            try
            {
                var dolgozok = db.Dolgozok.ToList();
                return Ok(dolgozok);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{felnev}")]
        public IHttpActionResult GetDolgozo(string felnev)
        {
            if (string.IsNullOrEmpty(felnev)) return BadRequest("Felhasználónév megadása kötelező.");

            try
            {
                var dolgozo = db.Dolgozok.Find(felnev);
                if (dolgozo == null) return NotFound();
                return Ok(dolgozo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostDolgozo([FromBody] Dolgozo dolgozo)
        {
            if (dolgozo == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            try
            {
                if (db.Dolgozok.Any(d => d.Felnev == dolgozo.Felnev))
                    return BadRequest("Ez a felhasználónév már foglalt.");

                db.Dolgozok.Add(dolgozo);
                db.SaveChanges();
                return Created($"api/Dolgozo/{dolgozo.Felnev}", dolgozo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{felnev}")]
        public IHttpActionResult PutDolgozo(string felnev, [FromBody] Dolgozo dolgozo)
        {
            if (dolgozo == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            if (felnev != dolgozo.Felnev)
                return BadRequest("Az URL paraméter nem egyezik a kérés törzsével.");

            try
            {
                var existing = db.Dolgozok.Find(felnev);
                if (existing == null) return NotFound();

                existing.Nev = dolgozo.Nev;
                existing.Jelszo = dolgozo.Jelszo;

                db.SaveChanges();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{felnev}")]
        public IHttpActionResult DeleteDolgozo(string felnev)
        {
            if (string.IsNullOrEmpty(felnev)) return BadRequest("Felhasználónév megadása kötelező.");

            try
            {
                var dolgozo = db.Dolgozok.Find(felnev);
                if (dolgozo == null) return NotFound();

                db.Dolgozok.Remove(dolgozo);
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
