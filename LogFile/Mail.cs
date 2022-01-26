
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

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

        }
        public void Send(string Subject, string Message, string MailFrom, string MailFromName, string MailTo, string[] attachments)
        {
            MailMessage m = new MailMessage();
            m.From = new MailAddress(MailFrom, MailFromName);
            m.To.Add(new MailAddress(MailTo));
            m.Subject = Subject;
            m.Body = Message;
            m.IsBodyHtml = true;
            foreach (string attachment in attachments)
            {
                Attachment data = new Attachment(attachment, MediaTypeNames.Application.Octet);
                m.Attachments.Add(data);
            }
            SmtpClient smtp = new SmtpClient(ServerAddress, ServerPort);
            smtp.Credentials = new NetworkCredential(ServerUser, ServerPassword);
            smtp.Send(m);
            smtp.Dispose();
            m.Dispose();
        }
        public void Send(string Subject, string Message, string MailFrom, string MailFromName, string[] MailTos, string[] attachments)
        {
            MailMessage m = new MailMessage();
            m.From = new MailAddress(MailFrom, MailFromName);
            foreach (string MailTo in MailTos)
            {
                m.To.Add(new MailAddress(MailTo));
            }
            m.Subject = Subject;
            m.Body = Message;
            m.IsBodyHtml = true;

            foreach (string attachment in attachments)
            {
                Attachment data = new Attachment(attachment, MediaTypeNames.Application.Octet);
                m.Attachments.Add(data);
            }

            SmtpClient smtp = new SmtpClient(ServerAddress, ServerPort);
            smtp.Credentials = new NetworkCredential(ServerUser, ServerPassword);
            smtp.Send(m);
            smtp.Dispose();
            m.Dispose();
        }
        public void Send(string Subject, string Message, string MailFrom, string MailFromName, string MailTo)
        {
            MailMessage m = new MailMessage();
            m.From = new MailAddress(MailFrom, MailFromName);
            m.To.Add(new MailAddress(MailTo));
            m.Subject = Subject;
            m.Body = Message;
            m.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient(ServerAddress, ServerPort);
            smtp.Credentials = new NetworkCredential(ServerUser, ServerPassword);
            smtp.Send(m);
            smtp.Dispose();
            m.Dispose();
        }
        public void Send(string Subject, string Message, string MailFrom, string MailFromName, string[] MailTos)
        {
            MailMessage m = new MailMessage();
            m.From = new MailAddress(MailFrom, MailFromName);
            foreach (string MailTo in MailTos)
            {
                m.To.Add(new MailAddress(MailTo));
            }
            m.Subject = Subject;
            m.Body = Message;
            m.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient(ServerAddress, ServerPort);
            smtp.Credentials = new NetworkCredential(ServerUser, ServerPassword);
            smtp.Send(m);
            smtp.Dispose();
            m.Dispose();
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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}