
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyLib
{
    public class Mail : IDisposable
    {
        public string ServerAddress;
        public int ServerPort;
        public string ServerUser;
        public string ServerPassword;
        private bool disposedValue;

        public Mail(string ServerAddress, string ServerUser, string ServerPassword, int ServerPort = 25)
        {
            this.ServerAddress = ServerAddress;
            this.ServerPort = ServerPort;
            this.ServerUser = ServerUser;
            this.ServerPassword = ServerPassword;

            //Send("", "", "", "", new string[] { "" });
        }


        public void Send(string subject, string message, string mailFrom, string mailFromName, string mailTo, string attachment = null)
        {
            var mailTos = new List<string> { mailTo };
            var attachments = string.IsNullOrEmpty(attachment) ? null : new List<string> { attachment };

            SendMail(mailFrom, mailFromName, subject, message, mailTos, attachments).GetAwaiter().GetResult();            
        }
        
        [Obsolete("Цей метод застарів. Використовуйте метод Send з параметром List<string> mailTos.")]
        public void Send(string subject, string message, string mailFrom, string mailFromName, string[] mailTos, string attachment = null)
        {
            var attachments = string.IsNullOrEmpty(attachment) ? null : new List<string> { attachment };

            SendMail(mailFrom, mailFromName, subject, message, new List<string>(mailTos), attachments).GetAwaiter().GetResult();
        }
        public void Send(string subject, string message, string mailFrom, string mailFromName, string mailTo, string[] attachments)
        {
            var mailTos = new List<string> { mailTo };

            SendMail(mailFrom, mailFromName, subject, message, mailTos, new List<string>(attachments)).GetAwaiter().GetResult();
        }
        public void Send(string subject, string message, string mailFrom, string mailFromName, string[] mailTos, string[] attachments)
        {
            SendMail(mailFrom, mailFromName, subject, message, new List<string>(mailTos), new List<string>(attachments)).GetAwaiter().GetResult();
        }

        public void Send(string subject, string message, string mailFrom, string mailFromName, string mailTo)
        {
            var mailTos = new List<string> { mailTo };
            
            SendMail(mailFrom, mailFromName, subject, message, mailTos, null).GetAwaiter().GetResult();
        }
        public void Send(string subject, string message, string mailFrom, string mailFromName, string[] mailTos)
        {
            SendMail(mailFrom, mailFromName, subject, message, new List<string>(mailTos), null).GetAwaiter().GetResult();
        }

        public async Task<bool> Send(string subject, string message, string mailFrom, string mailFromName, List<string> mailTos, string attachment = null)
        {
            var attachments = string.IsNullOrEmpty(attachment) ? null : new List<string> { attachment };

            return await SendMail(mailFrom, mailFromName, subject, message, new List<string>(mailTos), attachments);
        }
        public async Task<bool> Send(string subject, string message, string mailFrom, string mailFromName, string mailTo, List<string> attachments)
        {
            var mailTos = new List<string> { mailTo };

            return await SendMail(mailFrom, mailFromName, subject, message, mailTos, new List<string>(attachments));
        }
        public async Task<bool> Send(string subject, string message, string mailFrom, string mailFromName, List<string> mailTos, List<string> attachments)
        {
            return await SendMail(mailFrom, mailFromName, subject, message, new List<string>(mailTos), new List<string>(attachments));
        }

        public async Task<bool> SendRobotMail(string subject, string message, string mailTo, string attachment = null)
        {
            var mailTos = new List<string> { mailTo };
            var attachments = string.IsNullOrEmpty(attachment) ? null : new List<string> { attachment };

            return await SendMail("j-mp-kv-kras2o-b01@fozzy.ua", "Робот Фоззі-Фарм", subject, message, mailTos, attachments);
        }
        public async Task<bool> SendRobotMail(string subject, string message, List<string> mailTos, string attachment = null)
        {
            var attachments = string.IsNullOrEmpty(attachment) ? null : new List<string> { attachment };

            return await SendMail("j-mp-kv-kras2o-b01@fozzy.ua", "Робот Фоззі-Фарм", subject, message, mailTos, attachments);
        }
        public async Task<bool> SendRobotMail(string subject, string message, string mailTo, List<string> attachments)
        {
            var mailTos = new List<string> { mailTo };

            return await SendMail("j-mp-kv-kras2o-b01@fozzy.ua", "Робот Фоззі-Фарм", subject, message, mailTos, attachments);
        }
        public async Task<bool> SendRobotMail(string subject, string message, List<string> mailTos, List<string> attachments)
        {
            return await SendMail("j-mp-kv-kras2o-b01@fozzy.ua", "Робот Фоззі-Фарм", subject, message, mailTos, attachments);
        }

        private async Task<bool> SendMail(string mailFrom, string mailFromName, string subject, string message, List<string> mailTos, List<string> attachments)
        {
            MailMessage mailMessage = null;
            try
            {
                mailMessage = new MailMessage();
                
                mailMessage.From = new MailAddress(mailFrom, mailFromName);
                mailMessage.Subject = subject;
                mailMessage.Body = message;
                mailMessage.IsBodyHtml = true;

                foreach (var mailTo in mailTos)
                {
                    mailMessage.To.Add(new MailAddress(mailTo));
                }

                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        mailMessage.Attachments.Add(new Attachment(attachment));
                    }
                }

                using (var smtpClient = new SmtpClient(ServerAddress, ServerPort))
                {
                    smtpClient.Credentials = new NetworkCredential(ServerUser, ServerPassword);
                    await smtpClient.SendMailAsync(mailMessage);
                }
                

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                // Після завершення надсилання, очищаємо вкладення
                if (mailMessage?.Attachments != null)
                {
                    foreach (var attachment in mailMessage.Attachments)
                    {
                        attachment.Dispose();
                    }
                }
                mailMessage.Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
