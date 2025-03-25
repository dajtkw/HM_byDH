namespace HM_byDH.Models
{
    public class ActivityType //Loại hoạt động
    {
        public int Id { get; set; }
        public string Name { get; set; } // Ví dụ: Đi bộ, Chạy bộ, Đạp xe, Tập gym
        public double CaloriesPerMinute { get; set; } // Calo đốt cháy mỗi phút
    }
}
