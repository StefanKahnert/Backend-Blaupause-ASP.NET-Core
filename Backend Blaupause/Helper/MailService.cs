using Datac24_Online.DTOs.DBox;
using Datac24_Online.Helper.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Datac24_Online.Helper
{
    public class MailService
    {
        private static void sendMail(MailMessage mail, string smtpCient, string mailAdress, string password)
        {
            SmtpClient client = new SmtpClient(smtpCient);
            client.Credentials = new System.Net.NetworkCredential(mailAdress, password);

            try
            {
                client.Send(mail);
            }
            catch (Exception)
            {
                throw new HttpException(HttpStatusCode.OK, "-1");
            }
        }

        public static void sendMail(MailRequestDTO dto)
        {
            string from = Properties.Resources.mailAccountNoReply;
            if (!string.IsNullOrEmpty(dto.activityCode))
                from = dto.activityCode;
            from += Properties.Resources.address;

            MailMessage message = new MailMessage(from, dto.to);
            message.Subject = dto.subject;
            message.Body = HttpUtility.UrlDecode(dto.message);
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = false;

            sendMail(message, Properties.Resources.smtpClient, Properties.Resources.mailCredentialsUser, Properties.Resources.mailCredentialsPassword);
        }

    }
}
