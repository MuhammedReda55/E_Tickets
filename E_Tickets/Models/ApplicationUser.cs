using Microsoft.AspNetCore.Identity;

namespace E_Tickets.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
