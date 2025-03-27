using HM_byDH.Data;
using HM_byDH.Models.ViewModels.Weight;
using HM_byDH.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static HM_byDH.Models.WeightGoals;

namespace HM_byDH.Controllers
{
    public class HealthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HealthController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Trang chính hiển thị thông tin sức khỏe
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Lấy bản ghi cân nặng mới nhất từ WeightLogs
            var latestWeightLog = await _context.WeightLogs
                .Where(w => w.UserId == user.Id)
                .OrderByDescending(w => w.Date)
                .FirstOrDefaultAsync();

            double currentWeight = latestWeightLog?.Weight ?? user.Weight ?? 0;
            double heightInMeters = (user.Height ?? 0) / 100.0; // Chuyển từ cm sang mét

            var goal = await _context.WeightGoals
                .Where(g => g.UserId == user.Id && g.EndDate >= DateTime.Today)
                .FirstOrDefaultAsync();

            var model = new HealthDashboardViewModel
            {
                CurrentWeight = currentWeight,
                Height = heightInMeters,
                TargetWeight = goal?.TargetWeight ?? 0,
                BMI = CalculateBMI(currentWeight, heightInMeters),
                WeightLogs = await _context.WeightLogs
                    .Where(w => w.UserId == user.Id)
                    .OrderBy(w => w.Date)
                    .ToListAsync(),
                Goal = goal,
                DailyCaloriesTarget = goal?.DailyCaloriesTarget ?? 0,
                ProgressPercentage = CalculateProgress(currentWeight, goal?.TargetWeight ?? 0, goal?.InitialWeight ?? currentWeight)
            };

            // Thêm thông báo động viên
            double weightChange = currentWeight - (goal?.InitialWeight ?? currentWeight);
            if (weightChange < 0)
            {
                ViewBag.Motivation = $"Bạn đã giảm {Math.Abs(weightChange):F1} kg. Cố lên!";
            }
            else if (weightChange > 0)
            {
                ViewBag.Motivation = $"Bạn đã tăng {weightChange:F1} kg. Tiếp tục nỗ lực!";
            }

            return View(model);
        }

        // Đặt mục tiêu cân nặng (GET)
        [HttpGet]
        public async Task<IActionResult> SetWeightGoal()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Lấy cân nặng hiện tại từ bản ghi mới nhất
            var latestWeightLog = await _context.WeightLogs
                .Where(w => w.UserId == user.Id)
                .OrderByDescending(w => w.Date)
                .FirstOrDefaultAsync();

            double currentWeight = latestWeightLog?.Weight ?? user.Weight ?? 0;

            var model = new SetWeightGoalViewModel
            {
                CurrentWeight = currentWeight
            };
            return View(model);
        }

        // Đặt mục tiêu cân nặng (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetWeightGoal(SetWeightGoalViewModel model)
        {
            
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Lỗi validation: {error.ErrorMessage}");
                    }
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var days = (model.EndDate - DateTime.Today).Days;
            if (days <= 0)
            {
                ModelState.AddModelError("", "Ngày hoàn thành phải sau ngày hiện tại.");
                return View(model);
            }

            var weightChange = model.TargetWeight - model.CurrentWeight;
            var totalCaloriesNeeded = Math.Abs(weightChange) * 7700; // 1kg = 7700 calo
            var dailyCalories = totalCaloriesNeeded / days;

            var goal = new WeightGoal
            {
                UserId = user.Id,
                TargetWeight = model.TargetWeight,
                InitialWeight = model.CurrentWeight,
                StartDate = DateTime.Today,
                EndDate = model.EndDate,
                DailyCaloriesTarget = dailyCalories
            };

            try
            {
                _context.WeightGoals.Add(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lưu: {ex.Message}");
                return View(model);
            }

        }


        // Thêm nhật ký cân nặng (GET)
        [HttpGet]
        public IActionResult AddWeightLog()
        {
            var model = new AddWeightLogViewModel
            {
                Date = DateTime.Today // Giá trị mặc định là ngày hiện tại
            };
            return View(model);
        }

        // Thêm nhật ký cân nặng (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWeightLog(AddWeightLogViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var log = new WeightLog
            {
                UserId = user.Id,
                Date = model.Date,
                Weight = model.Weight
            };

            // Đồng bộ cân nặng vào hồ sơ người dùng
            user.Weight = model.Weight;
            await _userManager.UpdateAsync(user);

            _context.WeightLogs.Add(log);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Cập nhật chiều cao (POST)
        [HttpPost]
        public async Task<IActionResult> UpdateHeight(double height)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false, message = "Người dùng không tồn tại." });

            user.Height = height;
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Chiều cao đã được cập nhật." });
        }

        // Các hàm hỗ trợ
        private double CalculateBMI(double weight, double height)
        {
            return height > 0 ? weight / (height * height) : 0;
        }

        private double CalculateProgress(double currentWeight, double targetWeight, double initialWeight)
        {
            if (targetWeight == initialWeight) return currentWeight == targetWeight ? 100 : 0;
            var totalChange = targetWeight - initialWeight;
            var currentChange = currentWeight - initialWeight;
            return totalChange != 0 ? Math.Min(Math.Max((currentChange / totalChange) * 100, 0), 100) : 0;
        }
    }
}