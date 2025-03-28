using HM_byDH.Models;
using HM_byDH.Models.ViewModels.Authentication;
using HM_byDH.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HM_byDH.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, [FromServices] IEmailSender emailSender)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName, // Gán giá trị FullName
                    Gender = model.Gender, // Lưu giới tính
                    DateOfBirth = model.DateOfBirth // Lưu ngày sinh
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, protocol: Request.Scheme);

                    await emailSender.SendEmailAsync(
                        model.Email,
                        "Xác nhận email của bạn",
                        $"Vui lòng xác nhận email bằng cách nhấp <a href='{confirmationLink}'>vào đây</a>.");

                    return RedirectToAction("RegisterConfirmation", new { email = model.Email });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterConfirmation(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Thông tin không hợp lệ.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View("ConfirmEmail", model: user.Email);
            }

            return View("Error", new Models.ErrorViewModel { RequestId = "Xác nhận email thất bại." });
        }

        // Đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Vui lòng xác nhận email trước khi đăng nhập.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var userRedirect = await _userManager.FindByEmailAsync(model.Email);
                    // Kiểm tra vai trò Admin
                    if (await _userManager.IsInRoleAsync(userRedirect, "Admin"))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Đăng nhập không thành công. Kiểm tra lại email hoặc password");
            }
            return View(model);
        }

        // Đăng xuất
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // Quên mật khẩu
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, [FromServices] IEmailSender emailSender)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, protocol: Request.Scheme);

                await emailSender.SendEmailAsync(
                    model.Email,
                    "Đặt lại mật khẩu",
                    $"Vui lòng đặt lại mật khẩu bằng cách nhấp <a href='{callbackUrl}'>vào đây</a>.");

                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // Reset mật khẩu
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return BadRequest("Token hoặc email không hợp lệ.");
            }
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ResendConfirmationEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail(string email, [FromServices] IEmailSender emailSender)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || await _userManager.IsEmailConfirmedAsync(user))
            {
                return RedirectToAction("Login");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, protocol: Request.Scheme);

            await emailSender.SendEmailAsync(
                email,
                "Xác nhận email của bạn",
                $"Vui lòng xác nhận email bằng cách nhấp <a href='{confirmationLink}'>vào đây</a>.");

            return RedirectToAction("RegisterConfirmation", new { email });
        }

        [HttpGet]
        public async Task<IActionResult> CreateAdmin()
        {
            // Kiểm tra xem tài khoản admin đã tồn tại chưa
            var adminUser = await _userManager.FindByEmailAsync("admin@gmail.com");
            if (adminUser != null)
            {
                return Content("Tài khoản admin đã tồn tại.");
            }

            // Tạo người dùng admin mới
            adminUser = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                FullName = "Admin",
                EmailConfirmed = true, // Bỏ qua xác nhận email
                Gender = "Unknown",    // Giá trị mặc định
                DateOfBirth = new DateTime(2025, 1, 1) // Giá trị mặc định
            };

            var result = await _userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                // Gán vai trò "Admin" cho người dùng
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                return Content("Tài khoản admin đã được tạo thành công.");
            }

            // Hiển thị lỗi nếu có
            return Content("Không thể tạo tài khoản admin: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

    }
}
