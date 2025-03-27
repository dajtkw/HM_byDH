namespace HM_byDH.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Type { get; set; } // "UpdateWeight" (cập nhật cân nặng) hoặc "Exercise" (tập luyện)
        public DateTime Time { get; set; } // Thời gian nhắc nhở
    }
}
