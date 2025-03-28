using HM_byDH.Models.ViewModels.Weight;

namespace HM_byDH.Models.ViewModels.Home
{
    public class HomeViewModel
    {
        public bool IsLoggedIn { get; set; }
        public string UserName { get; set; }
        public double CurrentWeight { get; set; }
        public double TargetWeight { get; set; }
        public double DailyCaloriesTarget { get; set; }
        public double MaintenanceCalories { get; set; } // Calo duy trì
        public double ProgressPercentage { get; set; }
        public double TotalCaloriesBurnedThisWeek { get; set; }
        public List<WeightLogViewModel> WeightLogs { get; set; }

    }
}
