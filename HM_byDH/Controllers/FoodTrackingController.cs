using HM_byDH.Data;
using HM_byDH.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HM_byDH.Models.ViewModels.Meals;

namespace HM_byDH.Controllers
{
    [Authorize]
    public class FoodTrackingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FoodTrackingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date)
        {
            var user = await _userManager.GetUserAsync(User);
            var selectedDate = date ?? DateTime.Today;

            var foodEntries = await _context.FoodEntries
                .Where(fe => fe.UserId == user.Id && fe.Date.Date == selectedDate.Date)
                .Include(fe => fe.FoodItem)
                .ToListAsync();

            var waterIntakes = await _context.WaterIntakes
                .Where(wi => wi.UserId == user.Id && wi.Date.Date == selectedDate.Date)
                .ToListAsync();

            var weeklyFoodEntries = await _context.FoodEntries
                .Where(fe => fe.UserId == user.Id && fe.Date >= selectedDate.AddDays(-6) && fe.Date <= selectedDate)
                .Include(fe => fe.FoodItem)
                .ToListAsync();

            var model = new FoodTrackingViewModel
            {
                FoodEntries = foodEntries.Select(fe => new FoodEntryViewModel
                {
                    Id = fe.Id,
                    FoodName = fe.FoodItem.Name,
                    Quantity = fe.Quantity,
                    Calories = fe.FoodItem.Calories * fe.Quantity / 100,
                    Protein = fe.FoodItem.Protein * fe.Quantity / 100,
                    Fat = fe.FoodItem.Fat * fe.Quantity / 100,
                    Carb = fe.FoodItem.Carb * fe.Quantity / 100,
                    Date = fe.Date
                }).ToList(),
                WaterIntakes = waterIntakes.Select(wi => new WaterIntakeViewModel
                {
                    Id = wi.Id,
                    IsGoalMet = wi.IsGoalMet,
                    Date = wi.Date
                }).ToList(),
                SelectedDate = selectedDate,
                IsWaterGoalMet = waterIntakes.Any(wi => wi.IsGoalMet),
                WeeklySummary = weeklyFoodEntries
                    .GroupBy(fe => fe.Date.Date)
                    .ToDictionary(
                        g => g.Key,
                        g => new DailySummary
                        {
                            Calories = g.Sum(fe => fe.FoodItem.Calories * fe.Quantity / 100),
                            Protein = g.Sum(fe => fe.FoodItem.Protein * fe.Quantity / 100),
                            Fat = g.Sum(fe => fe.FoodItem.Fat * fe.Quantity / 100),
                            Carb = g.Sum(fe => fe.FoodItem.Carb * fe.Quantity / 100)
                        })
            };

            model.TotalCalories = model.FoodEntries.Sum(fe => fe.Calories);
            model.TotalProtein = model.FoodEntries.Sum(fe => fe.Protein);
            model.TotalFat = model.FoodEntries.Sum(fe => fe.Fat);
            model.TotalCarb = model.FoodEntries.Sum(fe => fe.Carb);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> FoodList()
        {
            var foodItems = await _context.FoodItems.ToListAsync();
            return View(foodItems);
        }

        [HttpGet]
        public async Task<IActionResult> AddFoodEntry()
        {
            var model = new AddFoodEntryViewModel
            {
                Date = DateTime.Now,
                FoodItems = await _context.FoodItems.ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFoodEntry(AddFoodEntryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var foodEntry = new FoodEntry
                {
                    UserId = user.Id,
                    FoodItemId = model.FoodItemId,
                    Quantity = model.Quantity,
                    Date = model.Date
                };
                _context.FoodEntries.Add(foodEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { date = model.Date });
            }
            model.FoodItems = await _context.FoodItems.ToListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditFoodEntry(int id)
        {
            var entry = await _context.FoodEntries
                .Include(fe => fe.FoodItem)
                .FirstOrDefaultAsync(fe => fe.Id == id && fe.UserId == _userManager.GetUserId(User));
            if (entry == null) return NotFound();

            var model = new EditFoodEntryViewModel
            {
                Id = entry.Id,
                FoodItemId = entry.FoodItemId,
                Quantity = entry.Quantity,
                Date = entry.Date,
                FoodItems = await _context.FoodItems.ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFoodEntry(EditFoodEntryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entry = await _context.FoodEntries
                    .FirstOrDefaultAsync(fe => fe.Id == model.Id && fe.UserId == _userManager.GetUserId(User));
                if (entry == null) return NotFound();

                entry.FoodItemId = model.FoodItemId;
                entry.Quantity = model.Quantity;
                entry.Date = model.Date;
                _context.FoodEntries.Update(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { date = model.Date });
            }
            model.FoodItems = await _context.FoodItems.ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFoodEntry(int id)
        {
            var entry = await _context.FoodEntries
                .FirstOrDefaultAsync(fe => fe.Id == id && fe.UserId == _userManager.GetUserId(User));
            if (entry != null)
            {
                _context.FoodEntries.Remove(entry);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddWaterIntake()
        {
            return View(new AddWaterIntakeViewModel { Date = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWaterIntake(AddWaterIntakeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var waterIntake = new WaterIntake
                {
                    UserId = user.Id,
                    Date = model.Date,
                    IsGoalMet = model.IsGoalMet
                };
                _context.WaterIntakes.Add(waterIntake);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { date = model.Date });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditWaterIntake(int id)
        {
            var intake = await _context.WaterIntakes
                .FirstOrDefaultAsync(wi => wi.Id == id && wi.UserId == _userManager.GetUserId(User));
            if (intake == null) return NotFound();

            var model = new EditWaterIntakeViewModel
            {
                Id = intake.Id,
                IsGoalMet = intake.IsGoalMet,
                Date = intake.Date
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWaterIntake(EditWaterIntakeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var intake = await _context.WaterIntakes
                    .FirstOrDefaultAsync(wi => wi.Id == model.Id && wi.UserId == _userManager.GetUserId(User));
                if (intake == null) return NotFound();

                intake.IsGoalMet = model.IsGoalMet; 
                intake.Date = model.Date;
                _context.WaterIntakes.Update(intake);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { date = model.Date });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWaterIntake(int id)
        {
            var intake = await _context.WaterIntakes
                .FirstOrDefaultAsync(wi => wi.Id == id && wi.UserId == _userManager.GetUserId(User));
            if (intake != null)
            {
                _context.WaterIntakes.Remove(intake);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            var settings = await _context.UserSettings.FirstOrDefaultAsync(us => us.UserId == user.Id)
                ?? new UserSettings { UserId = user.Id };
            var model = new UserSettingsViewModel
            {
                WaterReminderInterval = settings.WaterReminderInterval,
                DoNotDisturbStart = settings.DoNotDisturbStart,
                DoNotDisturbEnd = settings.DoNotDisturbEnd
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(UserSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var settings = await _context.UserSettings.FirstOrDefaultAsync(us => us.UserId == user.Id)
                    ?? new UserSettings { UserId = user.Id };

                settings.WaterReminderInterval = model.WaterReminderInterval;
                settings.DoNotDisturbStart = model.DoNotDisturbStart;
                settings.DoNotDisturbEnd = model.DoNotDisturbEnd;

                if (settings.Id == 0) _context.UserSettings.Add(settings);
                else _context.UserSettings.Update(settings);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SearchFood(string term)
        {
            var foods = await _context.FoodItems
                .Where(f => f.Name.Contains(term))
                .Select(f => new { f.Id, f.Name, f.Calories, f.Protein, f.Fat, f.Carb })
                .Take(10)
                .ToListAsync();
            return Json(foods);
        }
    }
}
