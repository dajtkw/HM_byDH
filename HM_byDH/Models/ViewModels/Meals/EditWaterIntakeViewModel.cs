using System.ComponentModel.DataAnnotations;

namespace HM_byDH.Models.ViewModels.Meals
{
    public class EditWaterIntakeViewModel
    {
        public int Id { get; set; }
        public bool IsGoalMet { get; set; }
        public DateTime Date { get; set; }
    }
}
