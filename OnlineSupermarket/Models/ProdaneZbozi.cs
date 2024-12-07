using System;

namespace OnlineSupermarket.Models
{
    public class ProdaneZbozi
    {
        public int Pocet { get; set; }
        public decimal Prodejnicena { get; set; }
        public int ZboziIdZbozi { get; set; }
        public int IdTranzakce { get; set; }
        public DateTime DatumVytvoreni { get; set; }
        public DateTime DatumAktualizace { get; set; }
    }
}