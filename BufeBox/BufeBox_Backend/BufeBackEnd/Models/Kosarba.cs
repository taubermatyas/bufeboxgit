using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BufeBackEnd.Models
{
    public class Kosarba
    {
        [Key, Column(Order = 0)]
        public int Kkod { get; set; }

        [Key, Column(Order = 1)]
        public int Tid { get; set; }

        public int Tmenny { get; set; }

        // Navigációk
        public virtual Kosar Kosar { get; set; }
        public virtual Termek Termek { get; set; }
    }

}