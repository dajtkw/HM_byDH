using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;



namespace HM_byDH.Models.ViewModels.Activities
{
    public class AddActivityEntryViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn hoạt động")]
        public int ActivityTypeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thời gian")]
        [Range(1, 1440, ErrorMessage = "Thời gian phải từ 1 đến 1440 phút")]
        public double Duration { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        public DateTime Date { get; set; }

        [ValidateNever]
        public List<ActivityType> ActivityTypes { get; set; }
    }
}
