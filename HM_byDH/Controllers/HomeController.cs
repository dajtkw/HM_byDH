using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HM_byDH.Data;
using HM_byDH.Models;
using HM_byDH.Models.ViewModels.Home;
using System.Diagnostics;
using HM_byDH.Models.ViewModels.Weight;

namespace HM_byDH.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
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
                double? weightFromLog = latestWeightLog?.Weight;
                double? userWeight = user.CurrentWeight;
                model.CurrentWeight = weightFromLog ?? userWeight ?? 0.0;
                // Lấy mục tiêu cân nặng và calo hàng ngày
                var goal = await _context.WeightGoals
                    .Where(g => g.UserId == user.Id && g.StartDate <= DateTime.Today && g.EndDate >= DateTime.Today)
                    .FirstOrDefaultAsync();
                if (goal != null)
                {
                    model.TargetWeight = goal.TargetWeight;
                    model.DailyCaloriesTarget = goal.DailyCaloriesTarget;
                    model.ProgressPercentage = CalculateProgress(model.CurrentWeight, goal.TargetWeight, goal.InitialWeight);
                }
                else
                {
                    model.TargetWeight = 0; // Giá trị mặc định
                    model.DailyCaloriesTarget = 0; // Giá trị mặc định
                    model.ProgressPercentage = 0;
                }

                // Phần còn lại của mã giữ nguyên...
            }
            return View(model);
        }

        private double CalculateProgress(double currentWeight, double targetWeight, double initialWeight)
        {
            if (targetWeight == initialWeight)
            {
                return currentWeight == targetWeight ? 100.0 : 0.0;
            }

            if (targetWeight < initialWeight) // Mục tiêu là giảm cân
            {
                if (currentWeight <= targetWeight)
                {
                    return 100.0;
                }
                else
                {
                    double totalWeightToLose = initialWeight - targetWeight;
                    double weightLostSoFar = initialWeight - currentWeight;
                    return Math.Max(0, Math.Min(100, (weightLostSoFar / totalWeightToLose) * 100));
                }
            }
            else // Mục tiêu là tăng cân
            {
                if (currentWeight >= targetWeight)
                {
                    return 100.0;
                }
                else
                {
                    double totalWeightToGain = targetWeight - initialWeight;
                    double weightGainedSoFar = currentWeight - initialWeight;
                    return Math.Max(0, Math.Min(100, (weightGainedSoFar / totalWeightToGain) * 100));
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}