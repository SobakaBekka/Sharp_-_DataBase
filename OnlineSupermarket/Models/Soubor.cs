using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Soubor
    {
        [Key]
        public int IdSoubor { get; set; }
        public string Nazev { get; set; }
        public byte[] FileData { get; set; }
        public string Pripona { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string TypSouboru { get; set; }
    }
}