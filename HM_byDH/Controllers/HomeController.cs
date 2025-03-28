using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HM_byDH.Data;
using HM_byDH.Models;
using HM_byDH.Models.ViewModels.Home;
using HM_byDH.Models.ViewModels.Weight;

namespace HM_byDH.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                model.IsLoggedIn = true;
                model.UserName = user.FullName;

                // Lấy cân nặng hiện tại từ bản ghi mới nhất
                var latestWeightLog = await _context.WeightLogs
                    .Where(w => w.UserId == user.Id)
                    .OrderByDescending(w => w.Date)
                    .FirstOrDefaultAsync();
                double currentWeight = latestWeightLog?.Weight ?? user.Weight ?? 0.0;
                model.CurrentWeight = currentWeight;

                double userWeight = currentWeight;
                double userHeight = user.Height ?? 0.0; // Nếu null thì mặc định = 0.0
                int userAge = user.Age; // Giả sử Age luôn có giá trị

                //Tính maintenance calories bằng công thức Mifflin-St Jeor

                double bmr = 0;
                if (userWeight > 0 && userHeight > 0 && userAge > 0)
                {
                    if (user.Gender == "Male")
                    {
                        bmr = (10 * userWeight + 6.25 * userHeight - 5 * userAge + 5);
                    }
                    else if (user.Gender == "Female")
                    {
                        bmr = (10 * userWeight + 6.25 * userHeight - 5 * userAge - 161);
                    }
                    // Nếu giới tính người dùng chưa xác định hợp lệ thì bmr giữ nguyên = 0
                }
                double maintenanceCalories = bmr * 1.2; // Giả định mức hoạt động ít (sedentary)
                model.MaintenanceCalories = maintenanceCalories;

                // Lấy mục tiêu cân nặng và tính daily calories target
                var goal = await _context.WeightGoals
                    .Where(g => g.UserId == user.Id && g.StartDate <= DateTime.Today && g.EndDate >= DateTime.Today)
                    .FirstOrDefaultAsync();
                if (goal != null)
                {
                    model.TargetWeight = goal.TargetWeight;
                    double adjustment = goal.DailyCaloriesTarget; // Giá trị điều chỉnh từ mục tiêu
                    if (goal.TargetWeight < goal.InitialWeight) // Giảm cân
                    {
                        model.DailyCaloriesTarget = maintenanceCalories - adjustment;
                    }
                    else // Tăng cân
                    {
                        model.DailyCaloriesTarget = maintenanceCalories + adjustment;
                    }
                    model.ProgressPercentage = CalculateProgress(currentWeight, goal.TargetWeight, goal.InitialWeight);
                }
                else
                {
                    model.TargetWeight = 0;
                    model.DailyCaloriesTarget = maintenanceCalories; // Không có mục tiêu thì dùng maintenance
                    model.ProgressPercentage = 0;
                }

                // Lấy tổng calo đốt cháy trong tuần
                var recentActivities = await _context.ActivityEntries
                    .Where(a => a.UserId == user.Id && a.Date >= DateTime.Today.AddDays(-7))
                    .ToListAsync();
                model.TotalCaloriesBurnedThisWeek = recentActivities.Sum(a => a.CaloriesBurned);

                // Lấy dữ liệu cho biểu đồ cân nặng
                var weightLogs = await _context.WeightLogs
                    .Where(w => w.UserId == user.Id)
                    .OrderBy(w => w.Date)
                    .ToListAsync();
                model.WeightLogs = weightLogs.Select(w => new WeightLogViewModel
                {
                    Date = w.Date,
                    Weight = w.Weight
                }).ToList();
            }
            return View(model);
        }

        private double CalculateProgress(double currentWeight, double targetWeight, double initialWeight)
        {
            if (targetWeight == initialWeight) return currentWeight == targetWeight ? 100.0 : 0.0;

            if (targetWeight < initialWeight) // Giảm cân
            {
                if (currentWeight <= targetWeight) return 100.0;
                double totalWeightToLose = initialWeight - targetWeight;
                double weightLostSoFar = initialWeight - currentWeight;
                return Math.Max(0, Math.Min(100, (weightLostSoFar / totalWeightToLose) * 100));
            }
            else // Tăng cân
            {
                if (currentWeight >= targetWeight) return 100.0;
                double totalWeightToGain = targetWeight - initialWeight;
                double weightGainedSoFar = currentWeight - initialWeight;
                return Math.Max(0, Math.Min(100, (weightGainedSoFar / totalWeightToGain) * 100));
            }
        }
    }
}