
using Backend_Blaupause.Helper.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Backend_Blaupause.Helper
{
    public class MailService
    {
        public static void sendMail(MailMessage mail, string smtpCient, string mailAdress, string password)
        {
            SmtpClient client = new SmtpClient(smtpCient);
            client.Credentials = new System.Net.NetworkCredential(mailAdress, password);

            try
            {
                client.Send(mail);
            }
            catch (Exception)
            {
                throw new HttpException(HttpStatusCode.NotFound, "Mail Adress not found");
            }
        }

    }
}
