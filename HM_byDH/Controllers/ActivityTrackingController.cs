using HM_byDH.Data;
using HM_byDH.Models;
using HM_byDH.Models.ViewModels.Activities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HM_byDH.Controllers
{
    [Authorize]
    public class ActivityTrackingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActivityTrackingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date)
        {
            var user = await _userManager.GetUserAsync(User);
            var selectedDate = date ?? DateTime.Today;

            var activityEntries = await _context.ActivityEntries
                .Where(ae => ae.UserId == user.Id && ae.Date.Date == selectedDate.Date)
                .Include(ae => ae.ActivityType)
                .ToListAsync();

            var dailyGoal = await _context.ActivityGoals
                .FirstOrDefaultAsync(ag => ag.UserId == user.Id && ag.GoalType == "Daily");

            var weeklyGoal = await _context.ActivityGoals
                .FirstOrDefaultAsync(ag => ag.UserId == user.Id && ag.GoalType == "Weekly");

            var model = new ActivityTrackingViewModel
            {
                ActivityEntries = activityEntries.Select(ae => new ActivityEntryViewModel
                {
                    Id = ae.Id,
                    ActivityName = ae.ActivityType.Name,
                    Duration = ae.Duration,
                    CaloriesBurned = ae.CaloriesBurned,
                    Date = ae.Date
                }).ToList(),
                SelectedDate = selectedDate,
                DailyGoal = dailyGoal != null ? new ActivityGoalViewModel
                {
                    GoalType = "Daily",
                    TargetCalories = dailyGoal.TargetCalories,
                    CurrentCalories = activityEntries.Sum(ae => ae.CaloriesBurned)
                } : null,
                WeeklyGoal = weeklyGoal != null ? new ActivityGoalViewModel
                {
                    GoalType = "Weekly",
                    TargetCalories = weeklyGoal.TargetCalories,
                    CurrentCalories = await _context.ActivityEntries
                        .Where(ae => ae.UserId == user.Id && ae.Date >= selectedDate.AddDays(-(int)selectedDate.DayOfWeek) && ae.Date <= selectedDate)
                        .SumAsync(ae => ae.CaloriesBurned)
                } : null
            };

            model.TotalCaloriesBurned = model.ActivityEntries.Sum(ae => ae.CaloriesBurned);
            return View(model);
        }

        //Vào trang thêm hoạt động
        [HttpGet]
        public async Task<IActionResult> AddActivityEntry()
        {
            var model = new AddActivityEntryViewModel
            {
                Date = DateTime.Today,
                ActivityTypes = await _context.ActivityTypes.ToListAsync()
            };
            return View(model);
        }

        
        [HttpPost]
        public async Task<IActionResult> AddActivityEntry(AddActivityEntryViewModel model)
        {
            Console.WriteLine($"ActivityTypeId: {model.ActivityTypeId}");   

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                // Ghi log hoặc hiển thị errors để kiểm tra
                Console.WriteLine(string.Join(", ", errors));
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("OK");
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account"); // Hoặc xử lý lỗi phù hợp
                }
                var activityEntry = new ActivityEntry
                {
                    UserId = user.Id,
                    ActivityTypeId = model.ActivityTypeId,
                    Duration = model.Duration,
                    Date = model.Date
                };
                
                try
                {
                    _context.ActivityEntries.Add(activityEntry);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { date = model.Date });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message); // Ghi log lỗi
                    ModelState.AddModelError("", "Có lỗi khi lưu dữ liệu.");
                    model.ActivityTypes = await _context.ActivityTypes.ToListAsync();
                    return View(model);
                }
            }

            model.ActivityTypes = await _context.ActivityTypes.ToListAsync();
            return View(model);
        }

        [HttpGet]
        public IActionResult SetActivityGoal()
        {
            return View(new SetActivityGoalViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SetActivityGoal(SetActivityGoalViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                
                var existingGoal = await _context.ActivityGoals
                    .FirstOrDefaultAsync(ag => ag.UserId == user.Id && ag.GoalType == model.GoalType);

                if (existingGoal != null)
                {
                    existingGoal.TargetCalories = model.TargetCalories;
                    _context.ActivityGoals.Update(existingGoal);
                }
                else
                {
                    var newGoal = new ActivityGoal
                    {
                        UserId = user.Id,
                        GoalType = model.GoalType,
                        TargetCalories = model.TargetCalories
                    };
                    _context.ActivityGoals.Add(newGoal);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
