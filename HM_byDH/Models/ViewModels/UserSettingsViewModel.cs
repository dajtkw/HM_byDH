namespace HM_byDH.Models.ViewModels
{
    public class UserSettingsViewModel
    {
        public int WaterReminderInterval { get; set; }
        public TimeSpan DoNotDisturbStart { get; set; }
        public TimeSpan DoNotDisturbEnd { get; set; }
    }
}
