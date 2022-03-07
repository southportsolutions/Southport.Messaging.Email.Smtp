﻿
namespace Southport.Messaging.Email.Smtp.Test;

public class SmtpOptions : ISmtpOptions
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Address { get; set; }
    public string FromAddress { get; set; }
    public int Port { get; set; }
    public string TestEmailAddresses { get; set; }
}