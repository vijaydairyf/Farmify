using System.ComponentModel.DataAnnotations;

namespace webapi.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "PHONE_MIN_LENGTH", MinimumLength = 10)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public int UserType { get; set; }

        public int OTP {get; set;}

        public string device_id {get; set;}
        public int device_type {get; set;} //1 - Android, 2 - IPhone, 3 - Windows
        public string device_version {get; set;} //lollypop, Kitkat
        public string app_version {get; set;} //0.0.0.0
        public float latitude {get; set;}
        public float longitude {get; set;}

        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword {get; set;}
    }
}