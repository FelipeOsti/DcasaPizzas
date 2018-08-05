using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using WebAppRoma.Models;
using System.Web.Http;
using System.Data.Common;

namespace WebAppRoma.Controllers
{
    [Authorize]
    public class FuncionarioController : ApiController
    {
        public static Funcionario getFuncionario(Usuario usuario)
        {
            DbDataReader qryFunc = null;
            Conexao con = Conexao.Instance(usuario.DS_EMAIL);
            try
            {
                string sdsSql = "select * from FO_FUNCI where CD_FUNC = " + usuario.CD_MATR+" and DT_DEMIS is null";                
                qryFunc = con.execQuery(sdsSql);

                string scdCargo = "";
                string sdsCCusto = "";
                int ncdEstab = 0;

                if (qryFunc.HasRows)
                {
                    scdCargo = qryFunc.GetString(qryFunc.GetOrdinal("CD_CARGO"));
                    sdsCCusto = Convert.ToString(qryFunc.GetInt32(qryFunc.GetOrdinal("CD_CCUSTO")));
                    ncdEstab = Convert.ToInt32(qryFunc.GetValue(qryFunc.GetOrdinal("CD_ESTAB")));
                }

                return new Funcionario() { CD_CARGO = scdCargo, CD_CCUSTO = sdsCCusto, CD_ESTAB = ncdEstab, CD_FUNC = usuario.CD_MATR };
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!qryFunc.IsClosed) qryFunc.Close();
                con.fechaCon();
            }
        }
    }
}
