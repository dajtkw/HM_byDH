using static HM_byDH.Models.WeightGoals;

namespace HM_byDH.Models.ViewModels.Weight
{
    public class HealthDashboardViewModel
    {
        public double Height { get; set; }
        public double CurrentWeight { get; set; }
        public double TargetWeight { get; set; }
        public double BMI { get; set; }
        public string HealthStatus { get; set; }
        public List<WeightLog> WeightLogs { get; set; }
        public string NutritionAdvice { get; set; }
        public string ExerciseAdvice { get; set; }
        public double WeightChange { get; set; }
        public double ProgressPercentage { get; set; }
        public WeightGoal Goal { get; set; } // Thêm thông tin mục tiêu
        public double DailyCaloriesTarget { get; set; } // Thêm thuộc tính này 
    }
}
