using System;

namespace OnlineSupermarket.Models
{
    public class Zbozi
    {
        public int IdZbozi { get; set; }
        public string Nazev { get; set; }
        public decimal AktualniCena { get; set; }
        public decimal? CenaZeKlubKartou { get; set; }
        public decimal Hmotnost { get; set; }
        public string Slozeni { get; set; }
        public int KategorieIdKategorie { get; set; }
        public DateTime DatumVytvoreni { get; set; }
        public DateTime DatumAktualizace { get; set; }
    }

}