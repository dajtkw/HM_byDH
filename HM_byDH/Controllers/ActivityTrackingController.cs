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
                    Date = ae.Date,
                    Intensity = ae.Intensity
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

            // Kiểm tra xem người dùng đã đạt được mục tiêu hay chưa
            if (model.DailyGoal != null)
            {
                model.IsDailyGoalAchieved = model.DailyGoal.CurrentCalories >= model.DailyGoal.TargetCalories;
            }
            if (model.WeeklyGoal != null)
            {
                model.IsWeeklyGoalAchieved = model.WeeklyGoal.CurrentCalories >= model.WeeklyGoal.TargetCalories;
            }

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

                // Lấy thông tin ActivityType từ database
                var activityType = await _context.ActivityTypes.FindAsync(model.ActivityTypeId);
                if (activityType == null)
                {
                    ModelState.AddModelError("", "Loại hoạt động không hợp lệ.");
                    model.ActivityTypes = await _context.ActivityTypes.ToListAsync();
                    return View(model);
                }

                

                double caloriesBurned = CalculateCaloriesBurned(model.Duration, activityType.CaloriesPerMinute, model.Intensity);

                var activityEntry = new ActivityEntry
                {
                    UserId = user.Id,
                    ActivityTypeId = model.ActivityTypeId,
                    Duration = model.Duration,
                    Date = model.Date,
                    Intensity = model.Intensity, 
                    CaloriesBurned = caloriesBurned // Lưu giá trị calo đốt cháy
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
                    
                }
            }
            // Nếu không hợp lệ, điền lại danh sách ActivityTypes
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

        [HttpGet]
        public async Task<IActionResult> EditActivityEntry(int id)
        {
            var entry = await _context.ActivityEntries
                .Include(ae => ae.ActivityType) 
                .FirstOrDefaultAsync(ae => ae.Id == id);

            if (entry == null) return NotFound();

            var model = new EditActivityEntryViewModel
            {
                Id = entry.Id,
                ActivityTypeId = entry.ActivityTypeId,
                Duration = entry.Duration,
                Date = entry.Date,
                Intensity = entry.Intensity,
                CaloriesBurned = entry.CaloriesBurned,
                ActivityTypes = await _context.ActivityTypes.ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditActivityEntry(EditActivityEntryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entry = await _context.ActivityEntries
                    .Include(ae => ae.ActivityType)
                    .FirstOrDefaultAsync(ae => ae.Id == model.Id);
                if (entry == null) return NotFound();

                if (entry.ActivityType == null)
                {
                    ModelState.AddModelError("", "Loại hoạt động không hợp lệ.");
                    model.ActivityTypes = await _context.ActivityTypes.ToListAsync();
                    return View(model);
                }

                double caloriesPerMinute = entry.ActivityType.CaloriesPerMinute;
                double caloriesBurned = CalculateCaloriesBurned(model.Duration, caloriesPerMinute, model.Intensity);

                entry.ActivityTypeId = model.ActivityTypeId;
                entry.Duration = model.Duration;
                entry.Date = model.Date;
                entry.Intensity = model.Intensity;
                entry.CaloriesBurned = caloriesBurned;

                _context.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            model.ActivityTypes = await _context.ActivityTypes.ToListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteActivityEntry(int id)
        {
            var entry = await _context.ActivityEntries.FindAsync(id);
            if (entry != null)
            {
                _context.ActivityEntries.Remove(entry);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


        public static double CalculateCaloriesBurned(double duration, double caloriesPerMinute, string intensity)
        {
            double intensityFactor = intensity switch
            {
                "Nhẹ" => 0.8,
                "Vừa" => 1.0,
                "Mạnh" => 1.2,
                _ => 1.0 // Giá trị mặc định nếu không hợp lệ
            };
            return duration * caloriesPerMinute * intensityFactor;
        }
    }
}
