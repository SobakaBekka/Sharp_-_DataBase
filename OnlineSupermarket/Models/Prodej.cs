using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Prodej
    {
        [Key]
        public int IdProdeje { get; set; }

        [Required]
        public DateTime Datum { get; set; } = DateTime.Now; // Set default to current date

        [Required]
        public decimal CelkovaCena { get; set; }

        [Required]
        public int ZboziIdZbozi { get; set; }

        public int? PlatbaIdTranzakce { get; set; }
    }
}
