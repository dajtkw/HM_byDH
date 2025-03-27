using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace HM_byDH.Models.ViewModels.Activities
{
    public class EditActivityEntryViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn hoạt động")]
        public int ActivityTypeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thời gian")]
        [Range(1, 1440, ErrorMessage = "Thời gian phải từ 1 đến 1440 phút")]
        public double Duration { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn mức cường độ")]
        public string Intensity { get; set; } // Ví dụ: "Nhẹ", "Vừa", "Mạnh"
        public double CaloriesBurned { get; set; }

        [ValidateNever]
        public List<ActivityType> ActivityTypes { get; set; }
    }
}
