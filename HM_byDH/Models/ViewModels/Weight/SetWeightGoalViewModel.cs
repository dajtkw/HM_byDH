using System.ComponentModel.DataAnnotations;

public class SetWeightGoalViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập cân nặng hiện tại")]
    public double CurrentWeight { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập cân nặng mục tiêu")]
    public double TargetWeight { get; set; }

    
    public DateTime EndDate { get; set; }
}