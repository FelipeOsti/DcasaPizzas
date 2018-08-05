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
    [RoutePrefix("api/CotacaoMoeda")]
    public class CotacaoMoedaController : ApiController
    {
        [HttpGet]
        public List<MoedaCotacao> GetCotacao(int ncdMoeda)
        {
            var con = Conexao.Instance(null);
            DbDataReader dataR = null;
            try
            {
                List<MoedaCotacao> lstMoedas = new List<MoedaCotacao>();
                
                var sdsSql = "select a.CD_MOEDA, a.DS_DESCRI, a.DS_SIGLA, b.DT_COTACAO, b.VL_COTACAO from " +
                    " FI_MOEDA a, FI_COTAC b where a.CD_MOEDA = b.CD_MOEDA and a.CD_MOEDA = "+ncdMoeda+" and "+
                    " b.DT_COTACAO >= (select max(DT_COTACAO) from FI_COTAC where CD_MOEDA = "+ncdMoeda+ ")-4  order by b.DT_COTACAO";
                dataR = con.execQuery(sdsSql);

                if (dataR.HasRows)
                {
                    while (dataR.Read())
                    {
                        string sdsMoeda = dataR.GetString(dataR.GetOrdinal("DS_DESCRI"));
                        string sdsSigla = dataR.GetString(dataR.GetOrdinal("DS_SIGLA"));
                        DateTime ddtCotac = dataR.GetDateTime(dataR.GetOrdinal("DT_COTACAO"));
                        double nvlCotacao = Convert.ToDouble(dataR.GetValue(dataR.GetOrdinal("VL_COTACAO")));

                        lstMoedas.Add(new MoedaCotacao { CD_MOEDA = ncdMoeda, DS_MOEDA = sdsMoeda, DS_SIGLA = sdsSigla, DT_COTACAO = ddtCotac, VL_COTACAO = nvlCotacao });
                    }
                }

                return lstMoedas;
            }
            catch
            {
                throw;
            }
            finally
            {
                if(dataR != null)
                {
                    if (!dataR.IsClosed) dataR.Close();
                }
                con.fechaCon();
            }
        }
    }
}
