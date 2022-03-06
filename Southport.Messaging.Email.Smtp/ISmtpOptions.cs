namespace Southport.Messaging.Email.Smtp
{
    public interface ISmtpOptions
    {
        string Username { get; set; }
        string Password { get; set; }
        string Address { get; set; }
        int Port { get; set; }
        string TestEmailAddresses { get; set; }
    }
}