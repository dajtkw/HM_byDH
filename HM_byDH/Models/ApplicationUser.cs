using Microsoft.AspNetCore.Identity;

namespace HM_byDH.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
