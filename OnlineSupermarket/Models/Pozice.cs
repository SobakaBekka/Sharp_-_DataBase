using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Pozice
    {
        [Key]
        public int IdPozice { get; set; }
        public string Nazev { get; set; }
        public decimal Mzda { get; set; }
    }
}
