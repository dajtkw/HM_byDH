namespace HM_byDH.Models.ViewModels.Admin
{
    public class UserDetailViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string Role { get; set; }
    }
}
