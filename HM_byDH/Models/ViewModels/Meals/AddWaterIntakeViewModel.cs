using System.ComponentModel.DataAnnotations;

namespace HM_byDH.Models.ViewModels.Meals
{
    public class AddWaterIntakeViewModel
    {
        public bool IsGoalMet { get; set; }
        public DateTime Date { get; set; }
    }
}
