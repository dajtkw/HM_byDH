using System.ComponentModel.DataAnnotations;

namespace HM_byDH.Models.ViewModels.Admin
{
    public class AddFoodItemViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public double Calories { get; set; }
        [Required]
        public double Protein { get; set; }
        [Required]
        public double Fat { get; set; }
        [Required]
        public double Carb { get; set; }
    }
}
