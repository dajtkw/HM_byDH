using System.ComponentModel.DataAnnotations;

namespace HM_byDH.Models.ViewModels.Profile
{
    public class EditProfileViewModel
    {
        [Display(Name = "Giới tính")]
        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public string Gender { get; set; } // Thêm giới tính

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [Range(0, 300, ErrorMessage = "Chiều cao phải từ 0 đến 300 cm.")]
        [Display(Name = "Chiều cao (cm)")]
        public double? Height { get; set; }

        [Range(0, 500, ErrorMessage = "Cân nặng phải từ 0 đến 500 kg.")]
        [Display(Name = "Cân nặng (kg)")]
        public double? Weight { get; set; }

    }
}
