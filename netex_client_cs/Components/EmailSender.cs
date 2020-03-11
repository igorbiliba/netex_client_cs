using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace netex_client_cs.Components
{
    public class EmailSender
    {
        public class EmailSettingsItem
        {
            public string email       { get; set; }
            public string name        { get; set; }
            public string password    { get; set; }
            public string smtpAddress { get; set; }
            public int    port        { get; set; }
        }

        public EmailSettingsItem settings;

        public string subject;
        public string to;
        public void Send(string msg)
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress(settings.email, settings.name);
            // кому отправляем
            MailAddress to = new MailAddress(this.to);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = subject;
            // текст письма
            m.Body = msg;
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient(settings.smtpAddress, settings.port);
            // логин и пароль
            smtp.Credentials = new NetworkCredential(settings.email, settings.password);
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}
