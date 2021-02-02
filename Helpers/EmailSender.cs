using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace InstantTutors.Helpers
{
    public static class EmailSender
    {
        public async static Task<bool> SendEmailAsync(string Subject, string Body, string MailTo = "instanttutorsww@gmail.com", bool IsBodyHtml = false)
        {
            var mail = new MailMessage();
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            mail.IsBodyHtml = true;
            mail.From = new MailAddress(smtpSection.Network.UserName, "Instant Tutors");
            //mail.To.Add(username);
            mail.To.Add(MailTo); //instanttutorsww@gmail.com
            mail.To.Add("tutorfl9@gmail.com");
            mail.Subject = Subject;
            mail.Body = Body;

            using (var smtp = new SmtpClient(smtpSection.Network.Host, smtpSection.Network.Port))
            {
                smtp.EnableSsl = smtpSection.Network.EnableSsl;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
                await smtp.SendMailAsync(mail);
            }
            return true;
        }
        public static Task SendEmail(string Subject, string Body, string MailTo = "instanttutorsww@gmail.com", bool IsBodyHtml = false)
        {
            var mail = new MailMessage();
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            mail.IsBodyHtml = true;
            mail.From = new MailAddress(smtpSection.Network.UserName, "Instant Tutors");
            mail.To.Add(MailTo); //instanttutorsww@gmail.com
            mail.To.Add("tutorfl9@gmail.com");
            mail.Subject = Subject;
            mail.Body = Body;

            using (var smtp = new SmtpClient(smtpSection.Network.Host, smtpSection.Network.Port))
            {
                smtp.EnableSsl = smtpSection.Network.EnableSsl;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
                smtp.SendMailAsync(mail);
            }
            return null;
        }
    }


}