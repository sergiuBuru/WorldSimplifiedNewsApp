using System.ComponentModel.DataAnnotations;

namespace WorldSimplifiedNewsApp.Models.DTOs
{
    public class UserRegisterRequestDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
