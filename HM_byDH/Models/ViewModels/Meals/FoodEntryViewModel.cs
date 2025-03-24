namespace HM_byDH.Models.ViewModels.Meals
{
    public class FoodEntryViewModel
    {
        public int Id { get; set; }
        public string FoodName { get; set; }
        public double Quantity { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carb { get; set; }
        public DateTime Date { get; set; }
    }
}
