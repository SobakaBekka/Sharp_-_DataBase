using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Soubor
    {
        [Key]
        public int IdSouboru { get; set; }  // відповідає колонці "IDSOUBORU"

        [Required]
        [StringLength(100)]
        public string Nazev { get; set; }  // відповідає колонці "NAZEV"

        [Required]
        public byte[] SouborData { get; set; }  // відповідає колонці "SOUBOR" (тип BLOB)

        [Required]
        [StringLength(10)]
        public string Pripona { get; set; }  // відповідає колонці "PRIPONA"

        [Required]
        public DateTime UploadDate { get; set; }  // відповідає колонці "UPLOADDATE"

        [Required]
        public DateTime ModifyDate { get; set; }  // відповідає колонці "MODIFYDATE"

        [Required]
        [StringLength(50)]
        public string TypSouboru { get; set; }  // відповідає колонці "TYPSOUBORU"

        [Required]
        public int RegisUzivatelIdRegisUzivatele { get; set; }  // відповідає колонці "REGISUZIVATEL_IDREGISUZIVATELU"

        // Навігаційна властивість для зв’язку з таблицею "REGISUZIVATEL"
        [ForeignKey("RegisUzivatelIdRegisUzivatele")]
        public virtual RegisUzivatel RegisUzivatel { get; set; }
    }
}
