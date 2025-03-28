using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using HM_byDH.Data;
using HM_byDH.Models;
using HM_byDH.Models.ViewModels.Admin;

namespace HM_byDH.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: /Admin/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // Danh sách người dùng
        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = _userManager.GetRolesAsync(u).Result.FirstOrDefault() ?? "User"
            }).ToList();
            return View(model);
        }

        // Chi tiết người dùng
        [HttpGet]
        public async Task<IActionResult> UserDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID người dùng không hợp lệ.");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserDetailViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Height = user.Height ?? 0, // Xử lý giá trị null
                Weight = user.Weight ?? 0, // Xử lý giá trị null
                Role = roles.FirstOrDefault() ?? "User"
            };
            return View(model);
        }

        // Chỉnh sửa role
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID người dùng không hợp lệ.");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new EditRoleViewModel
            {
                UserId = user.Id,
                CurrentRole = roles.FirstOrDefault() ?? "User"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Không thể xóa vai trò hiện tại.");
                return View(model);
            }

            var addResult = await _userManager.AddToRoleAsync(user, model.NewRole);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Không thể thêm vai trò mới.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Cập nhật vai trò thành công!";
            return RedirectToAction("UserList");
        }

        // Xóa người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID người dùng không hợp lệ.");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Xóa người dùng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa người dùng thất bại.";
            }
            return RedirectToAction("UserList");
        }

        // Thêm món ăn mới
        [HttpGet]
        public IActionResult AddFoodItem()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFoodItem(AddFoodItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var foodItem = new FoodItem
                {
                    Name = model.Name,
                    Calories = model.Calories,
                    Protein = model.Protein,
                    Fat = model.Fat,
                    Carb = model.Carb
                };
                _context.FoodItems.Add(foodItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm món ăn thành công!";
                return RedirectToAction("AddFoodItem"); // Giữ người dùng ở trang thêm món ăn để tiếp tục
            }
            return View(model);
        }

        // Thêm hoạt động thể chất mới
        [HttpGet]
        public IActionResult AddActivityType()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivityType(AddActivityTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var activityType = new ActivityType
                {
                    Name = model.Name,
                    CaloriesPerMinute = model.CaloriesPerMinute
                };
                _context.ActivityTypes.Add(activityType);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm hoạt động thể chất thành công!";
                return RedirectToAction("AddActivityType"); // Giữ người dùng ở trang thêm hoạt động
            }
            return View(model);
        }
    }
}