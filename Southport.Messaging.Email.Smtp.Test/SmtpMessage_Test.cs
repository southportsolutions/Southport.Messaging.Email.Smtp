using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Southport.Messaging.Email.Core.EmailAttachments;
using Southport.Messaging.Email.Core.Recipient;
using Xunit;
using Xunit.Abstractions;

namespace Southport.Messaging.Email.Smtp.Test;

public class SmtpMessageTest
{
    private readonly SmtpOptions _options;

    public SmtpMessageTest(ITestOutputHelper output)
    {
        _options = (SmtpOptions) Startup.GetOptions();
    }

    #region Simple Message

    [Fact]
    public async Task Send_Simple_Message()
    {
        var emailAddress = "test1@southport.solutions";
        var message = new SmtpMessage(_options);
        var responses = await message.SetFromAddress(_options.FromAddress)
            .AddToAddress(emailAddress)
            .SetSubject("Test Email")
            .SetText("This is a test email.").Send();
            

        foreach (var response in responses)
        {
            Assert.True(response.IsSuccessful);
            Assert.Equal(emailAddress, response.EmailRecipient.EmailAddress.Address);
        }
    }

    [Fact]
    public async Task Send_Simple_Attachment_Message()
    {
        var emailAddress = "test1@southport.solutions";
        var message = new SmtpMessage(_options);
        var responses = await message.SetFromAddress(_options.FromAddress)
            .AddToAddress(emailAddress)
            .SetSubject("Test Email")
            .AddAttachments(new EmailAttachment()
            {
                AttachmentFilename = "test.txt",
                AttachmentType = "text/plain", 
                Content = "Test attachment content."
            })
            .SetText("This is a test email.").Send();
            

        foreach (var response in responses)
        {
            Assert.True(response.IsSuccessful);
            Assert.Equal(emailAddress, response.EmailRecipient.EmailAddress.Address);
        }
    }

    #endregion

    #region Message With Sutstituions

    [Fact]
    public async Task Send_Message_Text_WithSubstitutions()
    {
        var emailAddress = new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>() {["FirstName"] = "Robert"});
        var message = new SmtpMessage(_options);
        var responses = await message.SetFromAddress(_options.FromAddress)
            .AddToAddress(emailAddress)
            .SetSubject("Test Email")
            .SetText("Dear {{FirstName}} This is a test email.").SubstituteAndSend();


        foreach (var response in responses)
        {
            Assert.True(response.IsSuccessful);
            Assert.Equal(emailAddress.EmailAddress.Address, response.EmailRecipient.EmailAddress.Address);
        }
    }

    [Fact]
    public async Task Send_Message_Html_WithSubstitutions()
    {
        var html = await File.ReadAllTextAsync(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates/Html.html"));
        var emailRecipients = new List<IEmailRecipient>()
        {
            new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>() {["FirstName"] = "Robert"}),
            new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>() {["FirstName"] = "David"})
        };

        var message = new SmtpMessage(_options);
        var responses = (await message.SetFromAddress(_options.FromAddress)
            .AddToAddresses(emailRecipients)
            .SetSubject("Test Email")
            .SetHtml(html).SubstituteAndSend()).ToList();


        for (var i = 0; i < responses.Count(); i++)
        {
            var response = responses.ElementAt(i);
            var recipient = emailRecipients.ElementAt(i);
            Assert.True(response.IsSuccessful);
            Assert.Equal(recipient.EmailAddress.Address, response.EmailRecipient.EmailAddress.Address);
        }
    }

    #endregion
}