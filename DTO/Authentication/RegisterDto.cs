using System.ComponentModel.DataAnnotations;

namespace FinalProject.DTO.Authentication
{
    public class RegisterDto
    {

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Address { get; set; }


    }
}
