using System;

namespace OnlineSupermarket.Models
{
    public class ZboziNaPulte
    {
        public int Pocet { get; set; }
        public int ZboziIdZbozi { get; set; }
        public int PultIdPultu { get; set; }
        public DateTime DatumVytvoreni { get; set; }
        public DateTime DatumAktualizace { get; set; }
    }
}


