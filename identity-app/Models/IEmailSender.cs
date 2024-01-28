﻿namespace identity_app.Models
{
    public interface IEMailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
