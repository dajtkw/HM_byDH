using System.ComponentModel.DataAnnotations;

namespace HM_byDH.Models.ViewModels
{
    public class AddWaterIntakeViewModel
    {
        [Range(1, 10000, ErrorMessage = "Lượng nước phải từ 1ml đến 10000ml")]
        public double Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
