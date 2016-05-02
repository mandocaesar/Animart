using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Animart.Portal.Extension
{
    public class GmailExtension
    {
        private SmtpClient _client;
        private readonly string _emailAddress;
        private readonly string _password;

        public GmailExtension(string emailAddress, string password)
        {
            _emailAddress = emailAddress;
            _password = password;
            InitializeSmtpClient();
        }

        private void InitializeSmtpClient()
        {
            _client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailAddress, _password)
            };
        }

        public bool SendMessage(string subject, string body, string receiverMail)
        {
            try
            {
                MailMessage message = new MailMessage();

                message.From = new MailAddress(_emailAddress);
                message.To.Add(receiverMail);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;

                _client.Send(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
