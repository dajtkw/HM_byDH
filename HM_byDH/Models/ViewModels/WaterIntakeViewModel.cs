namespace HM_byDH.Models.ViewModels
{
    public class WaterIntakeViewModel
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public bool IsGoalMet { get; set; }
        public DateTime Date { get; set; }
    }
}

