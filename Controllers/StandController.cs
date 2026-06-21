using Trabalho.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Trabalho.Controllers
{
    public class StandController : Controller
    {
        // ===== CLIENTES =====

        public ActionResult Clientes()
        {
            using (Sabadao db = new Sabadao())
            {
                List<cliente> lst = db.clientes.ToList();
                return View(lst);
            }
        }

        public JsonResult GetClienteAsync(int id)
        {
            using (Sabadao db = new Sabadao())
            {
                cliente cli = db.clientes.Find(id);
                if (cli == null)
                    return Json(new { msg = "erro" }, JsonRequestBehavior.AllowGet);

                db.Entry(cli).Reference("cliente1").Load();

                return Json(new
                {
                    idcli = cli.idcli,
                    nome = cli.nome,
                    datanasc = cli.datanasc.HasValue ? cli.datanasc.Value.ToString("dd/MM/yyyy") : "",
                    idade = cli.idade,
                    categoria = cli.categoria,
                    tutor = cli.cliente1 != null ? cli.cliente1.nome : "N/A",
                    foto = cli.photopath ?? "nophoto.png"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreateCliente()
        {
            using (Sabadao db = new Sabadao())
            {
                List<string> lstcats = new List<string>() { "alfa", "bravo", "charlie" };
                ViewBag.categorias = new SelectList(lstcats);
                ViewBag.tutores = new SelectList(db.clientes.ToList(), "idcli", "nome");
                return View(new cliente() { categoria = "Bravo" });
            }
        }

        [HttpPost]
        public ActionResult CreateCliente(cliente novo, HttpPostedFileBase fich)
        {
            using (Sabadao db = new Sabadao())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.clientes.Add(novo);
                        db.SaveChanges();

                        if (fich != null && fich.ContentLength > 0 && fich.ContentType.Contains("image"))
                        {
                            string ficheiro = novo.idcli.ToString() + System.IO.Path.GetExtension(fich.FileName);
                            novo.photopath = ficheiro;
                            string path = Server.MapPath($"~/Fotos/{ficheiro}");
                            fich.SaveAs(path);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Clientes", new { msg = "OK" });
                    }

                    List<string> lstcats = new List<string>() { "alfa", "bravo", "charlie" };
                    ViewBag.categorias = new SelectList(lstcats);
                    ViewBag.tutores = new SelectList(db.clientes.ToList(), "idcli", "nome");
                    return View(novo);
                }
                catch (Exception)
                {
                    return RedirectToAction("Clientes", new { msg = "Erro BD" });
                }
            }
        }

        public ActionResult EditCliente(int? id)
        {
            using (Sabadao db = new Sabadao())
            {
                if (id == null) return RedirectToAction("Clientes");
                cliente cli = db.clientes.Find(id.Value);
                if (cli == null) return RedirectToAction("Clientes");

                List<string> lstcats = new List<string>() { "alfa", "bravo", "charlie" };
                ViewBag.categorias = new SelectList(lstcats, cli.categoria);
                ViewBag.tutores = new SelectList(db.clientes.ToList(), "idcli", "nome", cli.tutor);
                return View(cli);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCliente(cliente editado, HttpPostedFileBase fich)
        {
            using (Sabadao db = new Sabadao())
            {
                try
                {
                    cliente cli = db.clientes.Find(editado.idcli);
                    if (cli == null) return RedirectToAction("Clientes");

                    if (ModelState.IsValid)
                    {
                        cli.nome = editado.nome;
                        cli.datanasc = editado.datanasc;
                        cli.categoria = editado.categoria;
                        cli.tutor = editado.tutor;

                        if (fich != null && fich.ContentLength > 0 && fich.ContentType.Contains("image"))
                        {
                            string ficheiro = cli.idcli.ToString() + System.IO.Path.GetExtension(fich.FileName);
                            cli.photopath = ficheiro;
                            string path = Server.MapPath($"~/Fotos/{ficheiro}");
                            fich.SaveAs(path);
                        }

                        db.SaveChanges();
                        return RedirectToAction("Clientes", new { msg = "OK" });
                    }

                    List<string> lstcats = new List<string>() { "alfa", "bravo", "charlie" };
                    ViewBag.categorias = new SelectList(lstcats, editado.categoria);
                    ViewBag.tutores = new SelectList(db.clientes.ToList(), "idcli", "nome", editado.tutor);
                    return View(editado);
                }
                catch (Exception)
                {
                    return RedirectToAction("Clientes", new { msg = "Erro BD" });
                }
            }
        }

        public ActionResult DetailsCliente(int? id)
        {
            using (Sabadao db = new Sabadao())
            {
                if (id == null) return RedirectToAction("Clientes");
                cliente cli = db.clientes.Find(id.Value);
                if (cli == null) return RedirectToAction("Clientes");

                db.Entry(cli).Reference("cliente1").Load();
                return View(cli);
            }
        }

        public ActionResult DeleteClientAsync(int? id)
        {
            using (Sabadao db = new Sabadao())
            {
                try
                {
                    cliente morto = db.clientes.Find(id ?? -1);
                    if (morto != null)
                    {
                        db.clientes.Remove(morto);
                        db.SaveChanges();
                        return Json(new { msg = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { msg = "erro" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new { msg = "erro bd" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        // ===== CARROS =====

        public ActionResult Carros()
        {
            using (Sabadao db = new Sabadao())
            {
                List<carro> lst = db.carros.ToList();
                return View(lst);
            }
        }

        public JsonResult GetCarroAsync(int id)
        {
            using (Sabadao db = new Sabadao())
            {
                carro c = db.carros.Find(id);
                if (c == null)
                    return Json(new { msg = "erro" }, JsonRequestBehavior.AllowGet);

                return Json(new
                {
                    idcar = c.idcar,
                    modelo = c.modelo,
                    phora = c.phora.HasValue ? c.phora.Value.ToString("0.00") : "0.00",
                    foto = c.photopath ?? "nophotocar.png"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreateCarro()
        {
            return View(new carro());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCarro(carro novo, HttpPostedFileBase fich)
        {
            using (Sabadao db = new Sabadao())
            {
                try
                {
                    var phoraStr = Request.Form["phora"]?.Replace(",", ".");
                    if (decimal.TryParse(phoraStr, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out decimal phoraVal))
                    {
                        novo.phora = phoraVal;
                        ModelState.Remove("phora");
                    }

                    if (ModelState.IsValid)
                    {
                        db.carros.Add(novo);
                        db.SaveChanges();

                        if (fich != null && fich.ContentLength > 0 && fich.ContentType.Contains("image"))
                        {
                            string ficheiro = "car_" + novo.idcar.ToString() + System.IO.Path.GetExtension(fich.FileName);
                            novo.photopath = ficheiro;
                            string path = Server.MapPath($"~/Fotos/{ficheiro}");
                            fich.SaveAs(path);
                            db.SaveChanges();
                        }

                        return RedirectToAction("Carros", new { msg = "OK" });
                    }
                    return View(novo);
                }
                catch (Exception)
                {
                    return RedirectToAction("Carros", new { msg = "Erro BD" });
                }
            }
        }

        public ActionResult EditCarro(int? id)
        {
            using (Sabadao db = new Sabadao())
            {
                if (id == null) return RedirectToAction("Carros");
                carro c = db.carros.Find(id.Value);
                if (c == null) return RedirectToAction("Carros");
                return View(c);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCarro(carro editado, HttpPostedFileBase fich)
        {
            using (Sabadao db = new Sabadao())
            {
                try
                {
                    var phoraStr = Request.Form["phora"]?.Replace(",", ".");
                    if (decimal.TryParse(phoraStr, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out decimal phoraVal))
                    {
                        editado.phora = phoraVal;
                        ModelState.Remove("phora");
                    }

                    carro c = db.carros.Find(editado.idcar);
                    if (c == null) return RedirectToAction("Carros");

                    if (ModelState.IsValid)
                    {
                        c.modelo = editado.modelo;
                        c.phora = editado.phora;

                        if (fich != null && fich.ContentLength > 0 && fich.ContentType.Contains("image"))
                        {
                            string ficheiro = "car_" + c.idcar.ToString() + System.IO.Path.GetExtension(fich.FileName);
                            c.photopath = ficheiro;
                            string path = Server.MapPath($"~/Fotos/{ficheiro}");
                            fich.SaveAs(path);
                        }

                        db.SaveChanges();
                        return RedirectToAction("Carros", new { msg = "OK" });
                    }
                    return View(editado);
                }
                catch (Exception)
                {
                    return RedirectToAction("Carros", new { msg = "Erro BD" });
                }
            }
        }

        public ActionResult DetailsCarro(int? id)
        {
            using (Sabadao db = new Sabadao())
            {
                if (id == null) return RedirectToAction("Carros");
                carro c = db.carros.Find(id.Value);
                if (c == null) return RedirectToAction("Carros");
                return View(c);
            }
        }

        public ActionResult DeleteCarroAsync(int? id)
        {
            using (Sabadao db = new Sabadao())
            {
                try
                {
                    carro c = db.carros.Find(id ?? -1);
                    if (c != null)
                    {
                        db.carros.Remove(c);
                        db.SaveChanges();
                        return Json(new { msg = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { msg = "erro" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new { msg = "erro bd" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        // ===== ALUGUERES =====

        public ActionResult Alugueres()
        {
            using (Sabadao db = new Sabadao())
            {
                List<aluguere> lst = db.alugueres.ToList();
                foreach (var a in lst)
                {
                    db.Entry(a).Reference("cliente").Load();
                    db.Entry(a).Reference("carro").Load();
                }
                return View(lst);
            }
        }

        public ActionResult ExportarAlugueresExcel()
        {
            using (Sabadao db = new Sabadao())
            {
                List<aluguere> lst = db.alugueres.ToList();

                foreach (var a in lst)
                {
                    db.Entry(a).Reference("cliente").Load();
                    db.Entry(a).Reference("carro").Load();
                }

                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Alugueres");

                    ws.Cell(1, 1).Value = "Cliente";
                    ws.Cell(1, 2).Value = "Carro";
                    ws.Cell(1, 3).Value = "Início";
                    ws.Cell(1, 4).Value = "Fim";
                    ws.Cell(1, 5).Value = "Tempo";
                    ws.Cell(1, 6).Value = "Custo";

                    int linha = 2;

                    foreach (var item in lst)
                    {
                        double custoCalculado = (item.tempo.HasValue && item.carro != null && item.carro.phora.HasValue)
                            ? Convert.ToDouble(item.tempo.Value) * Convert.ToDouble(item.carro.phora.Value)
                            : 0;

                        ws.Cell(linha, 1).Value = item.cliente != null ? item.cliente.nome : "";
                        ws.Cell(linha, 2).Value = item.carro != null ? item.carro.modelo : "";
                        ws.Cell(linha, 3).Value = item.inicio.HasValue ? item.inicio.Value.ToString("dd/MM/yyyy") : "";
                        ws.Cell(linha, 4).Value = item.fim.HasValue ? item.fim.Value.ToString("dd/MM/yyyy") : "";
                        ws.Cell(linha, 5).Value = item.tempo.HasValue ? item.tempo.Value : 0;
                        ws.Cell(linha, 6).Value = custoCalculado;
                        linha++;
                    }

                    var header = ws.Range(1, 1, 1, 6);
                    header.Style.Font.Bold = true;
                    header.Style.Fill.BackgroundColor = XLColor.LightGray;

                    ws.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "Alugueres.xlsx"
                        );
                    }
                }
            }
        }

        public ActionResult CreateAluguer()
        {
            using (Sabadao db = new Sabadao())
            {
                ViewBag.clientes = new SelectList(db.clientes.ToList(), "idcli", "nome");
                ViewBag.carros = new SelectList(db.carros.ToList(), "idcar", "modelo");
                return View(new aluguere());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAluguer(aluguere novo)
        {
            using (Sabadao db = new Sabadao())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.alugueres.Add(novo);
                        db.SaveChanges();
                        return RedirectToAction("Alugueres", new { msg = "OK" });
                    }
                    ViewBag.clientes = new SelectList(db.clientes.ToList(), "idcli", "nome", novo.idcli);
                    ViewBag.carros = new SelectList(db.carros.ToList(), "idcar", "modelo", novo.idcar);
                    return View(novo);
                }
                catch (Exception)
                {
                    return RedirectToAction("Alugueres", new { msg = "Erro BD" });
                }
            }
        }

        public ActionResult EditAluguer(int? id)
        {
            using (Sabadao db = new Sabadao())
            {
                if (id == null) return RedirectToAction("Alugueres");
                aluguere a = db.alugueres.Find(id.Value);
                if (a == null) return RedirectToAction("Alugueres");
                ViewBag.clientes = new SelectList(db.clientes.ToList(), "idcli", "nome", a.idcli);
                ViewBag.carros = new SelectList(db.carros.ToList(), "idcar", "modelo", a.idcar);
                return View(a);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAluguer(aluguere editado)
        {
            using (Sabadao db = new Sabadao())
            {
                try
                {
                    aluguere a = db.alugueres.Find(editado.idal);
                    if (a == null) return RedirectToAction("Alugueres");

                    if (ModelState.IsValid)
                    {
                        a.idcli = editado.idcli;
                        a.idcar = editado.idcar;
                        a.inicio = editado.inicio;
                        a.fim = editado.fim;
                        db.SaveChanges();
                        return RedirectToAction("Alugueres", new { msg = "OK" });
                    }
                    ViewBag.clientes = new SelectList(db.clientes.ToList(), "idcli", "nome", editado.idcli);
                    ViewBag.carros = new SelectList(db.carros.ToList(), "idcar", "modelo", editado.idcar);
                    return View(editado);
                }
                catch (Exception)
                {
                    return RedirectToAction("Alugueres", new { msg = "Erro BD" });
                }
            }
        }

        public ActionResult DetailsAluguer(int? id)
        {
            using (Sabadao db = new Sabadao())
            {
                if (id == null) return RedirectToAction("Alugueres");
                aluguere a = db.alugueres.Find(id.Value);
                if (a == null) return RedirectToAction("Alugueres");
                db.Entry(a).Reference("cliente").Load();
                db.Entry(a).Reference("carro").Load();
                return View(a);
            }
        }

        public ActionResult DeleteAluguerAsync(int? id)
        {
            using (Sabadao db = new Sabadao())
            {
                try
                {
                    aluguere a = db.alugueres.Find(id ?? -1);
                    if (a != null)
                    {
                        db.alugueres.Remove(a);
                        db.SaveChanges();
                        return Json(new { msg = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { msg = "erro" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new { msg = "erro bd" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}