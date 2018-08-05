using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using WebAppRoma.Models;

namespace WebAppRoma.Controllers
{
    [AllowAnonymous]
    [Authorize]
    [RoutePrefix("api/email")]
    public class EnviaEmailController : ApiController
    {
        public bool EnviaEmail(ConteudoEmail contEmail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtpsige.romagnole.local";
            client.EnableSsl = false;

            MailMessage mail = new MailMessage();

            mail.Sender = new MailAddress("suporte@romagnole.com.br", "Suporte AppRomagnole");
            mail.From = mail.Sender;
            mail.To.Add(new MailAddress(contEmail.destino, contEmail.destino));

            mail.Subject = contEmail.titulo;
            mail.Body = contEmail.conteudo;
            mail.IsBodyHtml = contEmail.isHTML;
            mail.Priority = MailPriority.High;

            try
            {
                client.Send(mail);
            }
            catch (Exception erro)
            {
                throw;
            }
            finally
            {
                mail = null;
            }

            return true;
        }
    }
}
