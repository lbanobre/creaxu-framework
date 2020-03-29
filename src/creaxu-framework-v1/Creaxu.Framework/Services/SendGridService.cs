using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Creaxu.Framework.Services
{
    public interface ISendGridService
    {
        Task Send(string email, string name, string templateName, object dynamicTemplateData, Dictionary<string, string> attachments = null);
        Task Send(string email, string subject, string body);
    }

    public class SendGridService : ISendGridService
    {
        private readonly IConfiguration _configuration;
        private readonly SendGridClient _sendGridClient;

        public SendGridService(IConfiguration configuration)
        {
            _configuration = configuration;
            _sendGridClient = new SendGridClient(_configuration["SendGrid:ApiKey"]);
        }

        public async Task Send(string email, string name, string templateName, object dynamicTemplateData, Dictionary<string, string> attachments = null)
        {
            var message = MailHelper.CreateSingleTemplateEmail(
                new EmailAddress(_configuration["SendGrid:From:Email"], _configuration["SendGrid:From:Name"]),
                new EmailAddress(email, name),
                _configuration[$"SendGrid:Templates:{templateName}"], 
                dynamicTemplateData);

            if(attachments != null)
            {
                foreach(var filename in attachments.Keys)
                {
                    message.AddAttachment(filename, attachments[filename]);
                }
            }

            await _sendGridClient.SendEmailAsync(message);            
        }

        public async Task Send(string email, string subject, string body)
        {
            var message = MailHelper.CreateSingleEmail(
                new EmailAddress(_configuration["SendGrid:From:Email"], _configuration["SendGrid:From:Name"]),
                new EmailAddress(email),
                subject, body, null);

            await _sendGridClient.SendEmailAsync(message);
        }
    }
}
