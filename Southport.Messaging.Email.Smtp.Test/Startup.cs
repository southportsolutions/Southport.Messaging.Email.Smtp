using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Southport.Messaging.Email.Smtp.Test;

public static class Startup
{
    public static ISmtpOptions Options { get; private set; }

    public static ISmtpOptions GetOptions()
    {
        if (Options == null)
        {
            var configurationBuilder = new ConfigurationBuilder()
                    
                .AddJsonFile(Path.Combine((new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent).ToString(), "appsettings.json"), true)
                .AddEnvironmentVariables();
            var config = configurationBuilder.Build();
            Options = new SmtpOptions();
            var section = config.GetSection("Mail");
            section.Bind(Options);

            if (string.IsNullOrWhiteSpace(Options.Address))
            {
                Options.Address = Environment.GetEnvironmentVariable("SMTPADDRESS");
                Options.Username = Environment.GetEnvironmentVariable("SMTPUSERNAME");
                Options.Password = Environment.GetEnvironmentVariable("SMTPPASSWORD");
                Options.Port = int.Parse(Environment.GetEnvironmentVariable("SMTPPORT"));
                Options.TestEmailAddresses = Environment.GetEnvironmentVariable("SMTPTESTEMAILADDRESSES");
            }

            if (string.IsNullOrEmpty(Options.Address))
            {
                throw new Exception("Unable to get the SMTP Address Key.");
            }
        }

        return Options;

    }
}