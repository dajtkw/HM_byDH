namespace HM_byDH.Models.ViewModels.Activities
{
    public class ActivityEntryViewModel
    {
        public int Id { get; set; }
        public string ActivityName { get; set; }
        public double Duration { get; set; }
        public double CaloriesBurned { get; set; }
        public DateTime Date { get; set; }
        public string Intensity { get; set; } 
    }
}
