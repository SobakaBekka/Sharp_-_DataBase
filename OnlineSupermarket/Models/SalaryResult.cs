using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class SalaryResult
    {
        [Key]
        public decimal Mzda { get; set; }
    }
}