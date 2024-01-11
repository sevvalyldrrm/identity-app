using Microsoft.AspNetCore.Identity;

namespace identity_app.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
