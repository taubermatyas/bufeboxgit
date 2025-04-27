using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using BufeBackEnd.Models;
using System.Text.RegularExpressions;

namespace BufeBackEnd.Controllers
{
    [RoutePrefix("api/Vasarlo")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class VasarloController : ApiController
    {
        private BufeContext db = new BufeContext();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetVasarlok()
        {
            try
            {
                var vasarlok = db.Vasarlok.ToList();
                return Ok(vasarlok);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{email}")]
        public IHttpActionResult GetVasarlo(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email megadása kötelező.");

            try
            {
                var vasarlo = db.Vasarlok.Find(email);
                if (vasarlo == null) return NotFound();
                return Ok(vasarlo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostVasarlo([FromBody] Vasarlo vasarlo)
        {
            if (vasarlo == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            try
            {
                if (db.Vasarlok.Any(v => v.Email == vasarlo.Email))
                    return BadRequest("Ez az email már regisztrálva van.");

                db.Vasarlok.Add(vasarlo);
                db.SaveChanges();
                return Created($"api/Vasarlo/{vasarlo.Email}", vasarlo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{email}")]
        public IHttpActionResult PutVasarlo(string email, [FromBody] Vasarlo vasarlo)
        {
            if (vasarlo == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            if (email != vasarlo.Email) return BadRequest("Az URL paraméter nem egyezik a kérés törzsével.");

            try
            {
                var existing = db.Vasarlok.Find(email);
                if (existing == null) return NotFound();

                existing.Nev = vasarlo.Nev;
                if (!string.IsNullOrEmpty(vasarlo.Jelszo))
                    existing.Jelszo = vasarlo.Jelszo;

                db.SaveChanges();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{email}")]
        public IHttpActionResult DeleteVasarlo(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email megadása kötelező.");

            try
            {
                var vasarlo = db.Vasarlok.Find(email);
                if (vasarlo == null) return NotFound();

                db.Vasarlok.Remove(vasarlo);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login([FromBody] dynamic loginData)
        {
            if (loginData == null) return BadRequest("Érvénytelen kérés törzs.");

            string email = loginData?.email?.ToString();
            string jelszo = loginData?.jelszo?.ToString();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(jelszo))
                return BadRequest("Email és jelszó megadása kötelező!");

            try
            {
                var vasarlo = db.Vasarlok.FirstOrDefault(v => v.Email == email && v.Jelszo == jelszo);
                if (vasarlo == null)
                    return Unauthorized();

                return Ok(new { success = true, nev = vasarlo.Nev });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("Update")]
        public IHttpActionResult UpdateVasarlo([FromBody] Vasarlo vasarlo)
        {
            if (vasarlo == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            try
            {
                var existing = db.Vasarlok.Find(vasarlo.Email);
                if (existing == null) return NotFound();

                existing.Nev = vasarlo.Nev;
                if (!string.IsNullOrEmpty(vasarlo.Jelszo))
                    existing.Jelszo = vasarlo.Jelszo;

                db.SaveChanges();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("Register")]
        public IHttpActionResult Register([FromBody] dynamic regData)
        {
            if (regData == null) return BadRequest("Érvénytelen kérés törzs.");

            string nev = regData?.nev?.ToString();
            string email = regData?.email?.ToString();
            string jelszo = regData?.jelszo?.ToString();

            if (string.IsNullOrEmpty(nev) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(jelszo))
                return BadRequest("Név, email és jelszó megadása kötelező!");

            // Password validation
            if (jelszo.Length < 8)
                return BadRequest("A jelszónak legalább 8 karakter hosszúnak kell lennie!");
            if (!Regex.IsMatch(jelszo, @"[A-Z]"))
                return BadRequest("A jelszónak tartalmaznia kell legalább egy nagybetűt!");
            if (!Regex.IsMatch(jelszo, @"[a-z]"))
                return BadRequest("A jelszónak tartalmaznia kell legalább egy kisbetűt!");
            if (!Regex.IsMatch(jelszo, @"[!@#$%^&*(),.?""':;{}]"))
                return BadRequest("A jelszónak tartalmaznia kell legalább egy különleges karaktert!");

            try
            {
                if (db.Vasarlok.Any(v => v.Email == email))
                    return BadRequest("Ez az email már regisztrálva van!");

                var vasarlo = new Vasarlo
                {
                    Email = email,
                    Nev = nev,
                    Jelszo = jelszo
                };

                db.Vasarlok.Add(vasarlo);
                db.SaveChanges();
                return Ok(new { success = true });
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