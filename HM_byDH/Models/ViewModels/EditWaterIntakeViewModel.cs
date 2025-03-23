using System.ComponentModel.DataAnnotations;

namespace HM_byDH.Models.ViewModels
{
    public class EditWaterIntakeViewModel
    {
        public int Id { get; set; }
        [Range(1, 10000)]
        public double Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
