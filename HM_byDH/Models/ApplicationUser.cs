using Microsoft.AspNetCore.Identity;

namespace HM_byDH.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; } // Ngày sinh
        public double? Height { get; set; } // Chiều cao (cm)
        public double? Weight { get; set; } 
    }
}
