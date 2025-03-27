namespace HM_byDH.Models.ViewModels.Activities
{
    public class ActivityTrackingViewModel
    {
        public List<ActivityEntryViewModel> ActivityEntries { get; set; }
        public ActivityGoalViewModel DailyGoal { get; set; }
        public ActivityGoalViewModel WeeklyGoal { get; set; }
        public double TotalCaloriesBurned { get; set; }
        public DateTime SelectedDate { get; set; }
        // Trạng thái hoàn thành
        public bool IsDailyGoalAchieved { get; set; }
        public bool IsWeeklyGoalAchieved { get; set; }
    }
}
