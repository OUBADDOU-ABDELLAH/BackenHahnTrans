using System.ComponentModel.DataAnnotations;

namespace HahnTransportAutomate.DTOs
{
    public class UserAuthenticationDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
