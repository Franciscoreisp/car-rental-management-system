using System.Linq;
using System.Web.Mvc;
using Trabalho.Models;

namespace Trabalho.Controllers
{
    public class LoginController : Controller
    {
        private Sabadao db = new Sabadao();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = db.utilizadors
                .FirstOrDefault(u => u.username == username && u.password == password);

            if (user != null)
            {
                Session["utilizador"] = user.nome;
                Session["username"] = user.username;
                return RedirectToAction("BemVindo", "Home");
            }

            ViewBag.Erro = "Utilizador ou password incorretos!";
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string nome, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Erro = "Preencha todos os campos.";
                return View();
            }

            var existe = db.utilizadors.FirstOrDefault(u => u.username == username);

            if (existe != null)
            {
                ViewBag.Erro = "Esse utilizador já existe!";
                return View();
            }

            utilizador novo = new utilizador();
            novo.nome = nome;
            novo.username = username;
            novo.password = password;

            db.utilizadors.Add(novo);
            db.SaveChanges();

            return RedirectToAction("Login", "Login");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }
    }
}