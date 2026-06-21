using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using Trabalho.Models;

namespace Trabalho.Controllers
{
    public class NewsletterController : Controller
    {
        private Sabadao db = new Sabadao();

        [HttpGet]
        public ActionResult Newsletter()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Newsletter(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    ViewBag.Msg = "Introduza um email válido.";
                    return View();
                }

                bool existe = db.newsletters.Any(x => x.email == email);

                if (existe)
                {
                    ViewBag.Msg = "Esse email já está subscrito na newsletter.";
                    return View();
                }

                newsletter n = new newsletter();
                n.email = email;
                n.datasubscricao = DateTime.Now;

                db.newsletters.Add(n);
                db.SaveChanges();

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("standprojeto.ti3@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Subscrição confirmada";
                mail.Body = "O seu email foi subscrito com sucesso na newsletter da página.";
                mail.IsBodyHtml = false;

                SmtpClient smtp = new SmtpClient();
                smtp.Send(mail);

                ViewBag.Msg = "Subscrição efetuada com sucesso. Foi enviado um email de confirmação.";
            }
            catch (Exception ex)
            {
                ViewBag.Msg = "Erro: " + ex.Message;
            }

            return View();
        }
    }
}