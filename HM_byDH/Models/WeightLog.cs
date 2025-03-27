namespace HM_byDH.Models
{
    public class WeightLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime Date { get; set; }
        public double Weight { get; set; } // Cân nặng tại thời điểm (kg)
    }
}
