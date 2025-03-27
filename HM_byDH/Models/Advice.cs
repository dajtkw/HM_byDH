namespace HM_byDH.Models
{
    public class Advice
    {
        public int Id { get; set; }
        public string Goal { get; set; } // "Reduce" hoặc "Increase"
        public string Type { get; set; } // "Nutrition" hoặc "Exercise"
        public string Content { get; set; } // Nội dung lời khuyên
    }
}
