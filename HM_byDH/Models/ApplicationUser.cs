using Microsoft.AspNetCore.Identity;

namespace HM_byDH.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; } // Ngày sinh
        public double? Height { get; set; } // Chiều cao (cm)
        public double? Weight { get; set; }
        public double CurrentWeight { get; set; } // Cân nặng hiện tại (kg)
        public string Gender { get; set; } // Ví dụ: "Male" hoặc "Female"
        

        // Thuộc tính tính toán tuổi
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                int age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}
