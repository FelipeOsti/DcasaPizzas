using AppRomagnole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppRoma.Models;

namespace WebAppRoma.Controllers
{
    [Authorize]
    public class GerenciarController : Controller
    {
        ConnectionMappingController conn;

        // GET: Gerenciar
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUsuariosAutorizados()
        {
            try
            {
                conn = new ConnectionMappingController();
                JsonResult json = new JsonResult();

                Dictionary<string, HashSet<string>> conexoes = new Dictionary<string, HashSet<string>>();
                conexoes = conn.GetConexoes();

                //conexoes.Add("351962083845467", new HashSet<string>() { "6964e784-699e-453c-8403-7a6b5d80e19a" });

                List<UsuarioConectado> lstUsuar = new List<UsuarioConectado>();
                int contador = 0;

                foreach (var c in conexoes)
                {
                    DispositivoAutorizadoController dispAuth = new DispositivoAutorizadoController();
                    var lstDisp = dispAuth.GetDispositivoIMEI(c.Key);

                    foreach (ModelDispositivo disp in lstDisp)
                    {
                        var usuario = UsuarioEmailController.getUsuario(disp.email);
                        contador++;

                        string[] lstId = c.Value.ToArray();
                        string id = string.Join(",", lstId);


                        lstUsuar.Add(new UsuarioConectado()
                        {
                            Index = contador,
                            ID = id,
                            Nome = usuario.NM_NOME,
                            IMEI = c.Key,
                            email = disp.email,
                            sdsDispositivo = disp.sdsDispositivo,
                            sdsVersao = disp.sdsVersao
                        });
                    }
                }

                json.Data = lstUsuar;
                return json;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}