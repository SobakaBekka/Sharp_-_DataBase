using System;

namespace OnlineSupermarket.Models
{
    public class Soubor
    {
        public int IDSOUBORU { get; set; }
        public string NAZEV_SOUBORU { get; set; }
        public string TYP_SOUBORU { get; set; }
        public string PRIPONA_SOUBORU { get; set; }
        public DateTime DATUM_NAHRANI { get; set; }
        public DateTime? DATUM_MODIFIKACE { get; set; }
        public byte[] OBSAH { get; set; }
        public int? ID_REGISUZIVATELU { get; set; }
    }
}