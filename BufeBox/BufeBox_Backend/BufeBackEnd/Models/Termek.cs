using System.ComponentModel.DataAnnotations;

namespace BufeBackEnd.Models
{
    public class Termek
    {
        [Key]
        public int Tid { get; set; }
        [Required]
        public string Tnev { get; set; }
        [Required]
        public int Mennyiseg { get; set; }
        [Required]
        public string Kiszereles { get; set; }
        [Required]
        public int Ar { get; set; }
        [Required]
        public int Afa { get; set; }
        [Required]
        public int Kid { get; set; }
        public string KepUrl { get; set; } // Új mező a kép URL-nek

        public virtual Kategoria Kategoria { get; set; }
    }
}