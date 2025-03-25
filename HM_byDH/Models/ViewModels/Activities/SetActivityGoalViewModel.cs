namespace HM_byDH.Models.ViewModels.Activities
{
    public class SetActivityGoalViewModel
    {
        public string GoalType { get; set; } // "Daily" hoặc "Weekly"
        public double TargetCalories { get; set; }
    }
}
