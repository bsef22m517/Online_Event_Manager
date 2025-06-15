using Microsoft.AspNetCore.Identity;

namespace EventHubApplication.Models
{
    public class AppUser : IdentityUser

    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public string UserRole { get; set; }
    }
}
