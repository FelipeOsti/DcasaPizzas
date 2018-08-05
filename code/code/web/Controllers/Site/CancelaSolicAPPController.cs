﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppRoma.Models;

namespace WebAppRoma.Controllers
{
    public class CancelaSolicAPPController : Controller
    {
        // GET: CancelaSolicAPP
        public ActionResult Index(float ID)
        {
            Conexao con = Conexao.Instance(null);
            try
            {
                if (ID > 0) {                    
                    con.ExecCommand("update IN_DISPOSITIVO set DT_CANCELACODIGO = sysdate, DT_INATIVO = sysdate where ID_DISPOSITIVO = " + ID);
                    return View();
                }
                else
                {
                    throw new Exception("Parametro ID Inválido!");
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                con.fechaCon();
            }
        }
    }
}