using System;
using System.Net.Mail;
using System.Web.Mvc;
using Trabalho.Models;

namespace Trabalho.Controllers
{
    public class EmailController : Controller
    {
        public ActionResult EnviarEmail()
        {
            return View(new EmailModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnviarEmail(EmailModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.To.Add(model.Para);
                    mail.Subject = model.Assunto;
                    mail.Body = model.Mensagem;
                    mail.IsBodyHtml = false;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Send(mail);
                    }
                }

                ViewBag.Msg = "Email enviado com sucesso.";
                ModelState.Clear();
                return View(new EmailModel());
            }
            catch (Exception ex)
            {
                ViewBag.Msg = "Erro ao enviar email: " + ex.Message;
                return View(model);
            }
        }
    }
}