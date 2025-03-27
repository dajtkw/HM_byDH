namespace HM_byDH.Models
{
    public class WeightGoals
    {
        public class WeightGoal
        {
            public int Id { get; set; }
            public string UserId { get; set; }
            public ApplicationUser User { get; set; }
            public double TargetWeight { get; set; } // Cân nặng mục tiêu (kg)
            public double InitialWeight { get; set; } // Cân nặng ban đầu (kg)
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public double DailyCaloriesTarget { get; set; }
        }
    }
}
