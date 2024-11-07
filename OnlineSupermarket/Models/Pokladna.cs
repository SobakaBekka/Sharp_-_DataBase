using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Pokladna
    {
        [Key]
        public int IdPokladny { get; set; }
        public int Cislo { get; set; }
        public int Samoobsluzna { get; set; }
    }
}
