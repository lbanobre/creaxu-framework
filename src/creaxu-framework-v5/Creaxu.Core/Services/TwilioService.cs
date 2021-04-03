using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Api.V2010.Account.AvailablePhoneNumberCountry;
using Twilio.Types;
using Twilio.Http;
using System.Linq;
using Twilio.Rest.Video.V1;
using Twilio.Jwt.AccessToken;

namespace Creaxu.Core.Services
{
    public interface ITwilioService
    {
        List<string> GetAvailableNumbers(int areaCode);
        void AssignNumber(string phone);
        MessageResource SendMessage(string to, string body);
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
            var replyUrl = _configuration["Twilio:MessageReceiverUrl"];

            IncomingPhoneNumberResource.Create(phoneNumber: new PhoneNumber(phone), smsMethod: HttpMethod.Post, smsUrl: new Uri(replyUrl));
        }

        public MessageResource SendMessage(string to, string body)
        {
            return SendMessage(_configuration["Twilio:PhoneService"], to, body, null);
        }

        public MessageResource SendMessage(string from, string to, string body)
        {
            return SendMessage(from, to, body, new Uri(_configuration["Twilio:StatusCallbackUrl"]));
        }

        public MessageResource SendMessage(string from, string to, string body, Uri statusCallback)
        {
            return MessageResource.Create(
                from: new PhoneNumber(from),
                to: new PhoneNumber(to),
                body: body.Replace(Environment.NewLine, "\n"),
                statusCallback: statusCallback);
        }
    }
}
