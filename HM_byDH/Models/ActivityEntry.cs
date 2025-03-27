namespace HM_byDH.Models
{
    public class ActivityEntry //Lịch sử hoạt động

    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ActivityTypeId { get; set; }
        public double Duration { get; set; } // Thời gian (phút)
        public DateTime Date { get; set; }
        public string Intensity { get; set; }
        public ApplicationUser User { get; set; }
        public ActivityType ActivityType { get; set; }
        public double CaloriesBurned { get; set; } // Calo đốt cháy
    }
}