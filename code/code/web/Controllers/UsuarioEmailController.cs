using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAppRoma.Models;

namespace WebAppRoma.Controllers
{
    [Authorize]
    public class UsuarioEmailController : ApiController
    {
        public static Usuario getUsuario(string sdsEmail)
        {
            DbDataReader qryUsuario = null;
            Conexao con = Conexao.Instance(sdsEmail);

            try
            {
                qryUsuario = con.execQuery("select * from IN_USUAR where DS_EMAIL = '"+sdsEmail+"'");

                string scdUsuar = "";
                string snmNome = "";
                int ncdMatr = 0;

                if (qryUsuario.HasRows)
                {
                    qryUsuario.Read();
                    scdUsuar = qryUsuario.GetString(qryUsuario.GetOrdinal("CD_USUAR"));
                    snmNome = qryUsuario.GetString(qryUsuario.GetOrdinal("NM_NOME"));
                    ncdMatr = Convert.ToInt32(qryUsuario.GetValue(qryUsuario.GetOrdinal("CD_MATR")));
                }
                else
                {
                    scdUsuar = sdsEmail.Substring(0,sdsEmail.IndexOf("@"));
                    qryUsuario = con.execQuery("select * from IN_USUAR where CD_USUAR = '" + scdUsuar.ToUpper() + "'");
                    if (qryUsuario.HasRows)
                    {
                        qryUsuario.Read();
                        scdUsuar = qryUsuario.GetString(qryUsuario.GetOrdinal("CD_USUAR")).Trim();
                        snmNome = qryUsuario.GetString(qryUsuario.GetOrdinal("NM_NOME"));
                        ncdMatr = Convert.ToInt32(qryUsuario.GetValue(qryUsuario.GetOrdinal("CD_MATR")));
                        sdsEmail = qryUsuario.GetString(qryUsuario.GetOrdinal("DS_EMAIL"));
                    }
                    else
                    {
                        return null;
                    }
                }

                return new Usuario { CD_MATR = ncdMatr, CD_USUARIO = scdUsuar, NM_NOME = snmNome, DS_EMAIL = sdsEmail };
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!qryUsuario.IsClosed) qryUsuario.Close();
                con.fechaCon();
            }
        }
    }
}
