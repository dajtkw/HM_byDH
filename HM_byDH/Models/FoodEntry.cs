namespace HM_byDH.Models
{
    public class FoodEntry
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int FoodItemId { get; set; }
        public double Quantity { get; set; }
        public DateTime Date { get; set; }
        public ApplicationUser User { get; set; }
        public FoodItem FoodItem { get; set; }
    }
}
