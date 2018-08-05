using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;

namespace DCasaPizzasWeb.Controllers
{
    public class EmailController : ApiController
    {
        public string EnviarEmail(string sdsDestino, string sdsTitulo, string sdsConteudo)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress("solarinegocios@gmail.com");
                    mail.To.Add(sdsDestino);

                    mail.Subject = sdsTitulo;
                    mail.IsBodyHtml = true;
                    mail.Body = sdsConteudo;

                    using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;

                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Credentials = new NetworkCredential("solarinegocios@gmail.com", "solarinegocios04");

                        smtp.Send(mail);

                        return "OK";
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
