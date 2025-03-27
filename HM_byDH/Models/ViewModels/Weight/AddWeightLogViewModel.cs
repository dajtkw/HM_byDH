using System.ComponentModel.DataAnnotations;

namespace HM_byDH.Models.ViewModels.Weight
{
    public class AddWeightLogViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập cân nặng")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        public DateTime Date { get; set; }
    }
}
