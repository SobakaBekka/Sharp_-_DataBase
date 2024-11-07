using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class PasswordChangeLog
    {
        [Key]
        public int UserID { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}