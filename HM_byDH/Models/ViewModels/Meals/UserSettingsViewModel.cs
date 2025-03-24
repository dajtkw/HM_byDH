namespace HM_byDH.Models.ViewModels.Meals
{
    public class UserSettingsViewModel
    {
        public int WaterReminderInterval { get; set; }
        public TimeSpan DoNotDisturbStart { get; set; }
        public TimeSpan DoNotDisturbEnd { get; set; }
    }
}
