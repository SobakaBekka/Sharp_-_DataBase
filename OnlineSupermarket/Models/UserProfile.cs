using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class UserProfile
    {
        public int IdRegisUzivatelu { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }
    }
}