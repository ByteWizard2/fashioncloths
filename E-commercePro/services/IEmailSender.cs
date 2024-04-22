using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

public interface IEmailSender
{
    Task<string> GenerateOTPAsync(int length = 6);
    Task SendEmailAsync(string email, string subject, string message);
}




