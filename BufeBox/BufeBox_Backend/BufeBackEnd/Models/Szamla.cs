using System.ComponentModel.DataAnnotations;

namespace BufeBackEnd.Models
{
    public class Szamla
    {
        [Key]
        public int Szkod { get; set; }
        [Required]
        public int Osszeg { get; set; }
        [Required]
        public string Email { get; set; }

        public virtual Vasarlo Vasarlo { get; set; }
    }
}