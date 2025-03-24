using System.ComponentModel.DataAnnotations;

namespace HM_byDH.Models.ViewModels.Meals
{
    public class EditFoodEntryViewModel
    {
        public int Id { get; set; }
        public int FoodItemId { get; set; }
        [Range(1, 10000)]
        public double Quantity { get; set; }
        public DateTime Date { get; set; }
        public List<FoodItem> FoodItems { get; set; }
    }
}
