using System;

namespace OnlineSupermarket.Models
{
    public class Sklad
    {
        public int IdSkladu { get; set; }
        public string NazevSkladu { get; set; }
        public int PocetPolicek { get; set; }
        public int? Plocha { get; set; }
        public DateTime DatumVytvoreni { get; set; }
        public DateTime DatumAktualizace { get; set; }
    }
}