﻿using HM_byDH.Data;
using HM_byDH.Hubs;
using HM_byDH.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HM_byDH.Services
{
    public class WaterReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<WaterReminderHub> _hubContext;

        public WaterReminderService(IServiceProvider serviceProvider, IHubContext<WaterReminderHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var now = DateTime.Now;
                var users = await userManager.Users.ToListAsync(stoppingToken);

                foreach (var user in users)
                {
                    var settings = await context.UserSettings.FirstOrDefaultAsync(us => us.UserId == user.Id, stoppingToken)
                        ?? new UserSettings { UserId = user.Id };

                    var currentTime = now.TimeOfDay;
                    var isDoNotDisturb = currentTime >= settings.DoNotDisturbStart || currentTime <= settings.DoNotDisturbEnd;
                    if (isDoNotDisturb) continue;

                    var lastReminder = now.AddHours(-settings.WaterReminderInterval);
                    var waterToday = await context.WaterIntakes
                        .AnyAsync(wi => wi.UserId == user.Id && wi.Date.Date == now.Date && wi.IsGoalMet, stoppingToken);

                    if (!waterToday && now.Minute == 0)
                    {
                        await _hubContext.Clients.User(user.Id)
                            .SendAsync("ReceiveWaterReminder", "Đã đến giờ nhắc nhở! Bạn chưa uống đủ nước hôm nay.");
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
