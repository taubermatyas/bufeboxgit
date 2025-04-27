using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BufeBackEnd.Models
{
    public class Kosar
    {
        [Key]
        public int Kkod { get; set; }
        [Required]
        public DateTime Idopont { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Felnev { get; set; }
        public string Megjegyzes { get; set; } // Nullable
        public bool Elkeszult { get; set; } // Már létezik, bool típusként

        public virtual Vasarlo Vasarlo { get; set; }
        public virtual Dolgozo Dolgozo { get; set; }
        public virtual ICollection<Kosarba> KosarbaTetelek { get; set; }
    }
}