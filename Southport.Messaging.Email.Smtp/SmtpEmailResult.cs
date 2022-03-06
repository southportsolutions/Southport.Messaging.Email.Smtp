using Southport.Messaging.Email.Core.Recipient;
using Southport.Messaging.Email.Core.Result;

namespace Southport.Messaging.Email.Smtp;

public class SmtpEmailResult : EmailResult
{
    public SmtpEmailResult(IEmailRecipient recipient, bool isSuccessful, string message, Exception exception) 
        : this(recipient, isSuccessful, message)
    {
        Exception = exception;
    }
    public SmtpEmailResult(IEmailRecipient recipient, bool isSuccessful, string message) : base(recipient, isSuccessful, message)
    {
    }
        
    public Exception Exception { get; set; }
}