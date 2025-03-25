namespace HM_byDH.Models
{
    public class ActivityGoal //Mục tiêu luyện tập
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string GoalType { get; set; } // "Daily" hoặc "Weekly"
        public double TargetCalories { get; set; } // Mục tiêu calo đốt cháy
        public ApplicationUser User { get; set; }
    }
}
