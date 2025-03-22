namespace HM_byDH.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Cấu hình SMTP (ví dụ dùng Gmail)
            var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new System.Net.NetworkCredential("btd1462004@gmail.com", "uuyg hafm mpfa gzxn"),
                EnableSsl = true
            };
            return client.SendMailAsync(new System.Net.Mail.MailMessage("your-email@gmail.com", email, subject, message) { IsBodyHtml = true });
        }
    }
}
