using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using BufeBackEnd.Models;

namespace BufeBackEnd.Controllers
{
    [RoutePrefix("api/Kosarba")]
    public class KosarbaController : ApiController
    {
        private BufeContext db = new BufeContext();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetKosarbaTetelek()
        {
            try
            {
                var tetelek = db.KosarbaTetelek.ToList();
                return Ok(tetelek);
            }
            catch (Exception ex)
            { 
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{kkod:int}/{tid:int}")]
        public IHttpActionResult GetKosarba(int kkod, int tid)
        {
            try
            {
                var kosarba = db.KosarbaTetelek.Find(kkod, tid);
                if (kosarba == null) return NotFound();
                return Ok(kosarba);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostKosarba([FromBody] Kosarba kosarba)
        {
            if (kosarba == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            try
            {
                var kosar = db.Kosarak.FirstOrDefault(k => k.Kkod == kosarba.Kkod);
                if (kosar == null) return BadRequest($"A kosár (Kkod: {kosarba.Kkod}) nem létezik.");

                var termek = db.Termekek.FirstOrDefault(t => t.Tid == kosarba.Tid);
                if (termek == null) return BadRequest($"A termék (Tid: {kosarba.Tid}) nem létezik.");

                if (kosarba.Tmenny <= 0)
                    return BadRequest("A mennyiségnek pozitívnak kell lennie.");

                // Készletellenőrzés: a kért mennyiség nem lehet nagyobb, mint az elérhető készlet
                if (termek.Mennyiseg < kosarba.Tmenny)
                    return BadRequest($"Nincs elegendő mennyiség a termékből ({termek.Tnev}). Elérhető: {termek.Mennyiseg}, kért: {kosarba.Tmenny}.");

                if (db.KosarbaTetelek.Any(k => k.Kkod == kosarba.Kkod && k.Tid == kosarba.Tid))
                    return BadRequest("Ez a tétel már létezik a kosárban. Használj PUT-ot a frissítéshez.");

                termek.Mennyiseg -= kosarba.Tmenny;
                if (termek.Mennyiseg < 0) // Biztosítjuk, hogy a készlet ne legyen negatív
                    return BadRequest("A készlet nem csökkenthető negatív értékre.");

                db.KosarbaTetelek.Add(kosarba);
                db.SaveChanges();
                return Created($"api/Kosarba/{kosarba.Kkod}/{kosarba.Tid}", kosarba);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{kkod:int}/{tid:int}")]
        public IHttpActionResult PutKosarba(int kkod, int tid, [FromBody] Kosarba kosarba)
        {
            if (kosarba == null) return BadRequest("Érvénytelen kérés törzs.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            if (kkod != kosarba.Kkod || tid != kosarba.Tid)
                return BadRequest("Az URL paraméterek nem egyeznek a kérés törzsével.");

            try
            {
                var existing = db.KosarbaTetelek.Find(kkod, tid);
                if (existing == null) return NotFound();

                var termek = db.Termekek.FirstOrDefault(t => t.Tid == kosarba.Tid);
                if (termek == null) return BadRequest($"A termék (Tid: {kosarba.Tid}) nem létezik.");

                // Számoljuk ki a szükséges mennyiségi változást
                int mennyisegValtozas = kosarba.Tmenny - existing.Tmenny;
                if (termek.Mennyiseg < mennyisegValtozas)
                    return BadRequest($"Nincs elegendő mennyiség a termékből ({termek.Tnev}). Elérhető: {termek.Mennyiseg}, kért további: {mennyisegValtozas}.");

                termek.Mennyiseg -= mennyisegValtozas;
                if (termek.Mennyiseg < 0) // Biztosítjuk, hogy a készlet ne legyen negatív
                    return BadRequest("A készlet nem csökkenthető negatív értékre.");

                existing.Tmenny = kosarba.Tmenny;

                db.SaveChanges();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{kkod:int}/{tid:int}")]
        public IHttpActionResult DeleteKosarba(int kkod, int tid)
        {
            try
            {
                var kosarba = db.KosarbaTetelek.Find(kkod, tid);
                if (kosarba == null) return NotFound();

                var termek = db.Termekek.FirstOrDefault(t => t.Tid == kosarba.Tid);
                if (termek != null)
                {
                    termek.Mennyiseg += kosarba.Tmenny; // Mennyiség visszaállítása
                }

                db.KosarbaTetelek.Remove(kosarba);
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