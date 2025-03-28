using System.ComponentModel.DataAnnotations;

namespace HM_byDH.Models.ViewModels.Admin
{
    public class AddActivityTypeViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public double CaloriesPerMinute { get; set; }
    }
}
