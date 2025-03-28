﻿using HM_byDH.Models.ViewModels;
using HM_byDH.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HM_byDH.Models.ViewModels.Profile;

namespace HM_byDH.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfileController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Role = roles.FirstOrDefault() ?? "User",
                Height = user.Height,
                Weight = user.Weight,
                Gender = user.Gender, // Thêm giới tính
                Age = user.Age        // Tuổi tự tính từ DateOfBirth
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditProfileViewModel
            {
                DateOfBirth = user.DateOfBirth,
                Height = user.Height,
                Weight = user.Weight,
                Gender = user.Gender // Thêm giới tính
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                user.DateOfBirth = (DateTime)model.DateOfBirth;
                user.Height = model.Height;
                user.Weight = model.Weight;
                user.Gender = model.Gender; // Lưu giới tính

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
    }
}