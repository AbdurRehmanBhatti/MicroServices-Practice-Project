using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Modals
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
