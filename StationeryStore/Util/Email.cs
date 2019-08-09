using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace StationeryStore.Util
{
    public class Email
    {
        private static string storeEmail = "sa48team5@gmail.com";
        private static string storePassword = "passTeam5word";
        public static void SendEmail(string receiverEmail, string subject, string body)
        {
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(storeEmail, storePassword);
            SmtpClient client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = credentials
            };

            MailMessage mm = new MailMessage(storeEmail, receiverEmail)
            {
                Subject = subject,
                Body = body
            };
            mm.IsBodyHtml = true;
            try
            {
                client.Send(mm);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}