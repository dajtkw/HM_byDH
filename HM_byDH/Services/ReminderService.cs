using HM_byDH.Data;

namespace HM_byDH.Services
{
    public class ReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ReminderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var reminders = context.Reminders
                        .Where(r => r.Time <= DateTime.Now)
                        .ToList();

                    foreach (var reminder in reminders)
                    {
                        // Gửi thông báo (email, SignalR, hoặc log)
                        Console.WriteLine($"Nhắc nhở cho {reminder.UserId}: {reminder.Type} lúc {reminder.Time}");
                        context.Reminders.Remove(reminder); // Xóa sau khi xử lý
                    }
                    await context.SaveChangesAsync();
                }
                await Task.Delay(60000, stoppingToken); // Kiểm tra mỗi phút
            }
        }
    }
}
