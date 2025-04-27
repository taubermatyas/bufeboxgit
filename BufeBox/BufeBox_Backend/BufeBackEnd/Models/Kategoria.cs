using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BufeBackEnd.Models
{
    public class Kategoria
    {
        [Key]
        public int Kid { get; set; }
        [Required]
        public string Knev { get; set; }
        public virtual ICollection<Termek> Termekek { get; set; }
    }
}