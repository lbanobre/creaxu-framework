using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Api.V2010.Account.AvailablePhoneNumberCountry;
using Twilio.Types;
using Twilio.Http;
using System.Linq;

namespace Creaxu.Framework.Services
{
    public interface ITwilioService
    {
        List<string> GetAvailableNumbers(int areaCode);
        void AssignNumber(string phone);
        MessageResource SendMessage(string from, string to, string body);
    }

    public class TwilioService : ITwilioService
    {
        private readonly IConfiguration _configuration;

        public TwilioService(IConfiguration configuration)
        {
            _configuration = configuration;

            TwilioClient.Init(_configuration["Twilio:AccountSID"], _configuration["Twilio:AuthToken"]);
        }

        public List<string> GetAvailableNumbers(int areaCode)
        {
            return LocalResource.Read("US", areaCode: areaCode, smsEnabled: true, limit: 5).Select(lr => lr.PhoneNumber.ToString()).ToList();
        }

        public void AssignNumber(string phone)
        {
            var replyUrl = _configuration["Twilio:ReplyUrl"];

            IncomingPhoneNumberResource.Create(phoneNumber: new PhoneNumber(phone), smsMethod: HttpMethod.Post, smsUrl: new Uri(replyUrl));
        }

        public MessageResource SendMessage(string from, string to, string body)
        {
            return MessageResource.Create(
                from: new PhoneNumber(from),
                to: new PhoneNumber(to),
                body: body.Replace(Environment.NewLine, "\n"),
                statusCallback: new Uri(_configuration["Twilio:StatusCallbackUrl"]));
        }
    }
}
