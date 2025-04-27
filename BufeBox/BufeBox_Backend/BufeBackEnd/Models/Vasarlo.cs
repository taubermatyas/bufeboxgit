using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BufeBackEnd.Models
{
    public class Vasarlo
    {
        [Key]
        public string Email { get; set; }
        [Required]
        public string Nev { get; set; }
        public string Jelszo { get; set; }

        public virtual ICollection<Szamla> Szamlak { get; set; }
        public virtual ICollection<Kosar> Kosarak { get; set; }
    }
}