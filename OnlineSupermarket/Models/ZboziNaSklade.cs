using System;

namespace OnlineSupermarket.Models
{
    public class ZboziNaSklade
    {
        public int Pocet { get; set; }
        public int ZboziIdZbozi { get; set; }
        public int SkladIdSkladu { get; set; }
        public int? AdresaIdAdresy { get; set; }
        public int KategorieIdKategorii { get; set; }
        public DateTime DatumVytvoreni { get; set; }
        public DateTime DatumAktualizace { get; set; }
    }
}