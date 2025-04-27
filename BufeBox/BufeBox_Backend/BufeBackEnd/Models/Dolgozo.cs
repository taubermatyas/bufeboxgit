using System.ComponentModel.DataAnnotations;

namespace BufeBackEnd.Models
{
    public class Dolgozo
    {
        [Key]
        public string Felnev { get; set; }
        [Required]
        public string Nev { get; set; }
        [Required]
        public string Jelszo { get; set; } // Hash-elt jelszó
    }
}