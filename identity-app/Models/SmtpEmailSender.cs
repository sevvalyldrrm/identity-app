using System.Net.Mail;
using System.Net;

namespace identity_app.Models
{
    public class SmtpEmailSender : IEMailSender
    {

        private readonly string? _host;

        private readonly int _port;

        private readonly bool _enableSSl;

        private readonly string? _username;
        private readonly string? _password;

        public SmtpEmailSender(string? host, int port, bool enableSSL, string? username, string? password)
        {
            _host = host;
            _port = port;
            _enableSSl = enableSSL;
            _username = username;
            _password = password;

        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = _enableSSl
            };

            return client.SendMailAsync(new MailMessage(_username ?? "", email, subject, message) { IsBodyHtml = true });

        }
    }
}
