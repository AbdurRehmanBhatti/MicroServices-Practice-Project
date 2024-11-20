using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Modals
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
