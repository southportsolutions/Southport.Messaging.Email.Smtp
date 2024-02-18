using System.Text;
using HandlebarsDotNet;
using MailKit;
using MimeKit;
using Southport.Messaging.Email.Core;
using Southport.Messaging.Email.Core.EmailAttachments;
using Southport.Messaging.Email.Core.Recipient;
using Southport.Messaging.Email.Core.Result;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Southport.Messaging.Email.Smtp
{
    public class SmtpMessage : ISmtpMessage
    {
        private readonly ISmtpOptions _options;
        private readonly List<Stream> _streams = new();

        #region FromAddress

        IEmailMessageCore IEmailMessageCore.AddCustomArguments(Dictionary<string, string> customArguments)
        {
            return AddCustomArguments(customArguments);
        }

        public IEmailAddress FromAddress { get; set; }

        public string From => FromAddress.ToString();

        public ISmtpMessage SetFromAddress(IEmailAddress emailAddress)
        {
            FromAddress = emailAddress;
            return this;
        }

        public ISmtpMessage SetFromAddress(string emailAddress, string name = null)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentNullException(nameof(emailAddress), "The email address is required");
            }

            return SetFromAddress(new EmailAddress(emailAddress, name));
        }

        #endregion

        #region ReplyToAddress

        public string ReplyTo { get; private set; }
        
        public ISmtpMessage SetReplyTo(string emailAddress)
        {
            ReplyTo = emailAddress;
            return this;
        }

        #endregion

        #region ToAddresses

        public IEnumerable<IEmailRecipient> ToAddresses { get; set; }

        public IEnumerable<IEmailRecipient> ToAddressesValid => ToAddresses.Where(e => e.EmailAddress.IsValid);
        public IEnumerable<IEmailRecipient> ToAddressesInvalid => ToAddresses.Where(e => !e.EmailAddress.IsValid);

        public ISmtpMessage AddToAddress(IEmailRecipient recipient)
        {
            ((List<IEmailRecipient>)ToAddresses).Add(recipient);
            return this;
        }

        public ISmtpMessage AddToAddress(string emailAddress, string name = null)
        {
            return AddToAddress(new EmailRecipient(emailAddress, name));
        }

        public ISmtpMessage AddToAddresses(List<IEmailRecipient> recipients)
        {
            ((List<IEmailRecipient>)ToAddresses).AddRange(recipients);
            return this;
        }

        #endregion

        #region CcAddresses

        public IEnumerable<IEmailAddress> CcAddresses { get; set; }

        public IEnumerable<IEmailAddress> CcAddressesValid => CcAddresses.Where(e => e.IsValid);
        public IEnumerable<IEmailAddress> CcAddressesInvalid => CcAddresses.Where(e => !e.IsValid);

        public ISmtpMessage AddCcAddress(IEmailAddress emailAddress)
        {
            ((List<IEmailAddress>)CcAddresses).Add(emailAddress);
            return this;
        }

        public ISmtpMessage AddCcAddress(string emailAddress, string name = null)
        {
            return AddCcAddress(new EmailAddress(emailAddress, name));
        }

        public ISmtpMessage AddCcAddresses(List<IEmailAddress> emailAddresses)
        {
            ((List<IEmailAddress>)CcAddresses).AddRange(emailAddresses);
            return this;
        }

        #endregion

        #region BccAddresses

        public IEnumerable<IEmailAddress> BccAddresses { get; set; }

        public IEnumerable<IEmailAddress> BccAddressesValid => BccAddresses.Where(e => e.IsValid);
        public IEnumerable<IEmailAddress> BccAddressesInvalid => BccAddresses.Where(e => !e.IsValid);

        public ISmtpMessage AddBccAddress(IEmailAddress emailAddress)
        {
            ((List<IEmailAddress>)BccAddresses).Add(emailAddress);
            return this;
        }

        public ISmtpMessage AddBccAddress(string emailAddress, string name = null)
        {
            return AddBccAddress(new EmailAddress(emailAddress, name));
        }

        public ISmtpMessage AddBccAddresses(List<IEmailAddress> emailAddresses)
        {
            ((List<IEmailAddress>)BccAddresses).AddRange(emailAddresses);
            return this;
        }

        #endregion

        #region Subject

        public string Subject { get; private set; }

        public ISmtpMessage SetSubject(string subject)
        {
            Subject = subject;
            return this;
        }

        #endregion

        #region Text

        public string Text { get; set; }

        public ISmtpMessage SetText(string text)
        {
            Text = text?.Trim() ?? "";
            return this;
        }

        #endregion

        #region HTML

        public string Html { get; set; }

        public ISmtpMessage SetHtml(string html)
        {
            Html = html?.Trim() ?? "";
            return this;
        }

        #endregion

        #region Attachments

        public List<IEmailAttachment> Attachments { get; set; }

        public ISmtpMessage AddAttachments(IEmailAttachment attachment)
        {
            Attachments.Add(attachment);
            return this;
        }

        public ISmtpMessage AddAttachments(List<IEmailAttachment> attachments)
        {
            Attachments = attachments;
            return this;
        }

        #endregion

        #region Template

        public string TemplateId { get; set; }

        public ISmtpMessage SetTemplate(string template)
        {
            TemplateId = template;
            return this;
        }

        #endregion

        #region TemplateVersion

        public string TemplateVersion { get; set; }

        public ISmtpMessage SetTemplateVersion(string templateVersion)
        {
            TemplateVersion = templateVersion;
            return this;
        }

        #endregion

        #region DeliveryTime

        public DateTime? DeliveryTime { get; set; }

        public ISmtpMessage SetDeliveryTime(DateTime deliveryTime)
        {
            DeliveryTime = deliveryTime;
            return this;
        }

        #endregion

        #region TestMode

        public bool? TestMode { get; set; }

        public ISmtpMessage SetTestMode(bool testMode)
        {
            TestMode = testMode;
            return this;
        }

        #endregion

        #region Tracking

        public bool Tracking { get; set; }

        public ISmtpMessage SetTracking(bool tracking)
        {
            Tracking = tracking;
            return this;
        }

        #endregion

        #region TrackingClicks

        public bool TrackingClicks { get; set; }

        public ISmtpMessage SetTrackingClicks(bool tracking)
        {
            TrackingClicks = tracking;
            return this;
        }

        #endregion

        #region TrackingOpens

        public bool TrackingOpens { get; set; }

        public ISmtpMessage SetTrackingOpens(bool tracking)
        {
            TrackingOpens = tracking;
            return this;
        }

        #endregion

        #region RequireTls

        public bool RequireTls { get; set; }

        public ISmtpMessage SetRequireTls(bool requireTls)
        {
            RequireTls = requireTls;
            return this;
        }

        #endregion

        #region SkipVerification

        public bool SkipVerification { get; set; }

        public ISmtpMessage SetSkipVerification(bool verification)
        {
            SkipVerification = verification;
            return this;
        }

        #endregion

        #region Custom Variables

        public Dictionary<string, string> CustomArguments { get; } = new();

        public SmtpMessage AddCustomVariable(string name, string value)
        {
            CustomArguments.Add(name, value);
            return this;
        }
        
        public ISmtpMessage AddCustomArgument(string key, string value)
        {
            throw new NotImplementedException();
        }

        public ISmtpMessage AddCustomArguments(Dictionary<string, string> customArguments)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Substitutions

        public Dictionary<string, object> Substitutions { get; } = new();

        public ISmtpMessage AddSubstitution(string key, object value)
        {
            Substitutions[key] = value;
            return this;
        }

        public ISmtpMessage AddSubstitutions(Dictionary<string, object> substitutions)
        {
            foreach (var substitution in substitutions)
            {
                Substitutions[substitution.Key] = substitution.Value;
            }

            return this;
        }

        #endregion

        #region Core Methods

        IEmailMessageCore IEmailMessageCore.SetFromAddress(string emailAddress, string name)
        {
            return SetFromAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.SetFromAddress(IEmailAddress emailAddress)
        {
            return SetFromAddress(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.AddToAddress(IEmailRecipient recipient)
        {
            return AddToAddress(recipient);
        }

        IEmailMessageCore IEmailMessageCore.AddToAddress(string emailAddress, string name)
        {
            return AddToAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.AddToAddresses(List<IEmailRecipient> recipients)
        {
            return AddToAddresses(recipients);
        }

        IEmailMessageCore IEmailMessageCore.AddCcAddress(IEmailAddress emailAddress)
        {
            return AddCcAddress(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.AddCcAddress(string emailAddress, string name)
        {
            return AddCcAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.AddCcAddresses(List<IEmailAddress> emailAddresses)
        {
            return AddCcAddresses(emailAddresses);
        }

        IEmailMessageCore IEmailMessageCore.AddBccAddress(IEmailAddress emailAddress)
        {
            return AddBccAddress(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.AddBccAddress(string emailAddress, string name)
        {
            return AddBccAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.AddBccAddresses(List<IEmailAddress> emailAddresses)
        {
            return AddBccAddresses(emailAddresses);
        }

        IEmailMessageCore IEmailMessageCore.SetSubject(string subject)
        {
            return SetSubject(subject);
        }

        IEmailMessageCore IEmailMessageCore.SetText(string text)
        {
            return SetText(text);
        }

        IEmailMessageCore IEmailMessageCore.SetHtml(string html)
        {
            return SetHtml(html);
        }

        IEmailMessageCore IEmailMessageCore.AddAttachments(IEmailAttachment attachment)
        {
            return AddAttachments(attachment);
        }

        IEmailMessageCore IEmailMessageCore.AddAttachments(List<IEmailAttachment> attachments)
        {
            return AddAttachments(attachments);
        }

        IEmailMessageCore IEmailMessageCore.SetTemplate(string template)
        {
            return SetTemplate(template);
        }

        IEmailMessageCore IEmailMessageCore.SetDeliveryTime(DateTime deliveryTime)
        {
            return SetDeliveryTime(deliveryTime);
        }

        IEmailMessageCore IEmailMessageCore.SetTestMode(bool testMode)
        {
            return SetTestMode(testMode);
        }

        IEmailMessageCore IEmailMessageCore.SetTracking(bool tracking)
        {
            return SetTracking(tracking);
        }

        IEmailMessageCore IEmailMessageCore.SetTrackingClicks(bool tracking)
        {
            return SetTrackingClicks(tracking);
        }

        IEmailMessageCore IEmailMessageCore.SetTrackingOpens(bool tracking)
        {
            return SetTrackingOpens(tracking);
        }

        IEmailMessageCore IEmailMessageCore.SetReplyTo(string emailAddress)
        {
            return SetReplyTo(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.AddCustomArgument(string key, string value)
        {
            return AddCustomArgument(key, value);
        }

        IEmailMessageCore IEmailMessageCore.AddSubstitution(string key, object value)
        {
            return AddSubstitution(key, value);
        }

        IEmailMessageCore IEmailMessageCore.AddSubstitutions(Dictionary<string, object> substitutions)
        {
            return AddSubstitutions(substitutions);
        }

        #endregion

        public SmtpMessage(ISmtpOptions options)
        {
            _options = options;
            ToAddresses = new List<IEmailRecipient>();
            CcAddresses = new List<IEmailAddress>();
            BccAddresses = new List<IEmailAddress>();
            Attachments = [];
        }

        #region Send

        public async Task<IEnumerable<IEmailResult>> Send(CancellationToken cancellationToken = default)
        {
            return await Send(false, cancellationToken);
        }

        private async Task<IEnumerable<SmtpEmailResult>> Send(bool substitute = false, CancellationToken cancellationToken = default)
        {
            if (FromAddress == null)
            {
                throw new EmailMessagingException("The from address is required.");
            }

            if (!ToAddressesValid.Any() && !CcAddressesValid.Any() && !BccAddressesValid.Any())
            {
                throw new EmailMessagingException("There must be at least 1 recipient.");
            }

            if (string.IsNullOrWhiteSpace(Html) && string.IsNullOrWhiteSpace(Text) && string.IsNullOrWhiteSpace(TemplateId))
            {
                throw new EmailMessagingException("The message must have a message or reference a template.");
            }

            var messages = GetMimeMessages(substitute);

            var results = new List<SmtpEmailResult>();
            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(_options.Address, _options.Port, cancellationToken: cancellationToken);
                await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);

                foreach (var (recipient, message) in messages)
                {
                    try
                    {
                        await client.SendAsync(message, cancellationToken);
                        results.Add(new SmtpEmailResult(recipient, true, ""));
                    }
                    catch (CommandException e)
                    {
                        results.Add(new SmtpEmailResult(recipient, false, e.Message, e));
                    }
                    catch (Exception e)
                    {
                        results.Add(new SmtpEmailResult(recipient, false, e.Message, e));
                    }
                }

                await client.DisconnectAsync(true, cancellationToken);
            }
            finally
            {
                foreach (var stream in _streams)
                {
                    await stream.DisposeAsync();
                }
            }

            return results;
        }

        public async Task<IEnumerable<SmtpEmailResult>> SubstituteAndSend(CancellationToken cancellationToken = default)
        {
            return await Send(true, cancellationToken);
        }

        #endregion

        #region GetMimeMessage

        private Dictionary<IEmailRecipient, MimeMessage> GetMimeMessages(bool substitute = false)
        {
            var contents = new Dictionary<IEmailRecipient, MimeMessage>();
            var toAddresses = GetTestAddresses(ToAddressesValid.ToList());

            foreach (var emailRecipient in toAddresses)
            {
                contents[emailRecipient] = GetMimeMessage(emailRecipient, substitute);
            }

            return contents;
        }

        private MimeMessage GetMimeMessage(IEmailRecipient emailRecipient, bool substitute = false)
        {

            // ReSharper disable once UseObjectOrCollectionInitializer
            var message = new MimeMessage();
            var bodyBuilder = new BodyBuilder();

            #region Addresses

            message.From.Add(GetMailboxAddress(FromAddress));
            message.To.Add(GetMailboxAddress(emailRecipient));
            message.Cc.AddRange(GetMailboxAddress(CcAddressesValid));
            message.Bcc.AddRange(GetMailboxAddress(BccAddressesValid));

            #endregion

            #region Subject

            message.Subject = Subject;

            #endregion

            #region Text/HTML

            var text = string.IsNullOrWhiteSpace(Html) ? Text : Html;
            text = substitute ? Substitute(text, emailRecipient.Substitutions) : text;

            if (string.IsNullOrWhiteSpace(Html)) bodyBuilder.TextBody = text;
            else bodyBuilder.HtmlBody = text;

            #endregion

            #region Attachments

            //global attachments
            foreach (var attachment in Attachments)
            {
                bodyBuilder.Attachments.Add(attachment.AttachmentFilename, GetStream(attachment.Content));
            }

            //recipient specific attachments
            foreach (var attachment in emailRecipient.Attachments)
            {
                bodyBuilder.Attachments.Add(attachment.AttachmentFilename, GetStream(attachment.Content));
            }

            #endregion

            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }

        #endregion

        #region Helpers

        private string Substitute(string text, Dictionary<string, object> substitutions)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            if (substitutions != null && substitutions.Any())
            {
                var compileFunc = Handlebars.Compile(text);
                text = compileFunc(substitutions);
            }

            return text;
        }

        private IEnumerable<IEmailRecipient> GetTestAddresses(IEnumerable<IEmailRecipient> toAddresses)
        {
            if (string.IsNullOrWhiteSpace(_options.TestEmailAddresses))
            {
                return toAddresses;
            }

            var testEmailAddresses = _options.TestEmailAddresses.Split(',');
            var toAddressesTemp = new List<IEmailRecipient>();
            foreach (var toAddress in toAddresses)
            {
                var customArgs = toAddress.CustomArguments;
                customArgs["IsTest"] = "true";

                toAddressesTemp.AddRange(testEmailAddresses.Select(emailAddress => new EmailRecipient(emailAddress.Trim(), substitutions: toAddress.Substitutions, customArguments: toAddress.CustomArguments, attachments: toAddress.Attachments)));
            }

            if (CcAddresses.Any())
            {
                CcAddresses = testEmailAddresses.Select(emailAddress => new EmailAddress(emailAddress.Trim()));
            }

            if (BccAddresses.Any())
            {
                BccAddresses = testEmailAddresses.Select(emailAddress => new EmailAddress(emailAddress.Trim()));
            }

            toAddresses = toAddressesTemp;

            return toAddresses;
        }

        private Stream GetStream(string content)
        {
            var stream = new MemoryStream();
            var sw = new StreamWriter(stream, Encoding.UTF8);
            sw.Write(content);
            sw.Flush(); //otherwise you are risking empty stream
            stream.Seek(0, SeekOrigin.Begin);
            _streams.Add(stream);
            return stream;
        }

        private MailboxAddress GetMailboxAddress(IEmailAddress address)
        {
            return new MailboxAddress(address.Name, address.Address);
        }

        private MailboxAddress GetMailboxAddress(IEmailRecipient recipient)
        {
            return new MailboxAddress(recipient.EmailAddress.Name, recipient.EmailAddress.Address);
        }

        private IEnumerable<InternetAddress> GetMailboxAddress(IEnumerable<IEmailAddress> emailAddresses)
        {
            return emailAddresses.Select(address => new MailboxAddress(address.Name, address.Address)).ToList();
        }

        #endregion
    }
}