using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace E_Tickets.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
        public string Address { get; set; }
        
        public string photo { get; set; }
    }
}
