using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class ZboziNaSklade
    {
        [Key]
        public int Pocet { get; set; }
        public int ZboziIdZbozi { get; set; }
        public int SkladIdSkladu { get; set; }
    }
}
