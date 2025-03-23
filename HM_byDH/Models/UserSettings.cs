namespace HM_byDH.Models
{
    public class UserSettings
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int WaterReminderInterval { get; set; } = 3; // Giờ, mặc định 3 tiếng
        public TimeSpan DoNotDisturbStart { get; set; } = TimeSpan.FromHours(22); // 22:00
        public TimeSpan DoNotDisturbEnd { get; set; } = TimeSpan.FromHours(7); // 07:00
        public ApplicationUser User { get; set; }
    }
}
