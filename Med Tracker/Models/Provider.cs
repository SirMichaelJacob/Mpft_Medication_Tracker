using System.ComponentModel.DataAnnotations;

namespace Med_Tracker.Models
{
    public class Provider
    {

        [Key]
        public int ProviderId { get; set; }

        [Required(ErrorMessage = "Name cannot be empty")]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter a valid email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MaxLength(256)]
        public string PasswordHash { get; set; }

        [Compare("PasswordHash", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Re-Password")]
        [DataType(DataType.Password)]
        [MaxLength(256)]
        public string Re_PasswordHash { get; set; }

        public string Confirmed { get; set; }

        public string RegToken { get; set; }

    }
}