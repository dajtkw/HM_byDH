namespace HM_byDH.Models.ViewModels.Meals
{
    public class FoodTrackingViewModel
    {
        public List<FoodEntryViewModel> FoodEntries { get; set; }
        public List<WaterIntakeViewModel> WaterIntakes { get; set; }
        public double TotalCalories { get; set; }
        public double TotalProtein { get; set; }
        public double TotalFat { get; set; }
        public double TotalCarb { get; set; }
        public DateTime SelectedDate { get; set; }
        public bool IsWaterGoalMet { get; set; }
        public double MaintenanceCalories { get; set; } // Calo duy trì
        public double DailyCaloriesTarget { get; set; } // Tổng calo cần thiết
        public Dictionary<DateTime, DailySummary> WeeklySummary { get; set; }
    }
}
