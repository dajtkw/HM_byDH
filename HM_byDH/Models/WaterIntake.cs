namespace HM_byDH.Models
{
    public class WaterIntake
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public bool IsGoalMet { get; set; } // Đánh dấu đã uống đủ 1.5L chưa
        public ApplicationUser User { get; set; }
    }
}
