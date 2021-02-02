using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace InstantTutors.Helpers
{
    public static class SMSSender
    {
        public static Task SMSSenderAsync(string Message, string SmsTo = null)
        {
            string twilioAccountSid = ConfigurationManager.AppSettings["TwilioAccountSid"].ToString();
            string twilioAuthToken = ConfigurationManager.AppSettings["TwilioAuthToken"].ToString();
            string twilioPhoneNumber = ConfigurationManager.AppSettings["TwilioPhoneNumber"].ToString();
            string twilioAdminNumber = ConfigurationManager.AppSettings["TwilioAdminNumber"].ToString();

            SmsTo = SmsTo == null ? twilioAdminNumber : SmsTo;
            TwilioClient.Init(twilioAccountSid, twilioAuthToken);

            var t = Task.Run(() => MessageResource.CreateAsync(
                body: Message,
                from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                to: new Twilio.Types.PhoneNumber(SmsTo)
            ));

            return t;
        }
    }


}