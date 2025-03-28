using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace HM_byDH.Models.ViewModels.Meals
{
    public class AddFoodEntryViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn món ăn")]
        public int FoodItemId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, 10000, ErrorMessage = "Số lượng phải từ 1g đến 10,000g")]
        public double Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        public DateTime Date { get; set; }

        [ValidateNever]
        public List<FoodItem> FoodItems { get; set; }
    }
}
