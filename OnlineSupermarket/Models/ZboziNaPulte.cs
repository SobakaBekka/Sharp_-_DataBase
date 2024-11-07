using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class ZboziNaPulte
    {
        [Key]
        public int Pocet { get; set; }
        public int ZboziIdZbozi { get; set; }
        public int PultIdPultu { get; set; }
        public int IdPultu { get; set; }
    }
}
