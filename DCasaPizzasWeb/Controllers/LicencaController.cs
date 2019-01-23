using DCasaPizzasWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DCasaPizzasWeb.Controllers
{
    [RoutePrefix("api/Licenca")]
    public class LicencaController : ApiController
    {
        [HttpGet]
        [Route("GetLicenca/{nidCliente}")]
        public List<IN_CHAVELICENCAModel> GetLicenca(long nidCliente)
        {
            var con = new Conexao();
            SqlDataReader qLicenca = null;
            try
            {
                var lista = new List<IN_CHAVELICENCAModel>();

                qLicenca = con.ExecQuery("select * from solari.IN_CHAVELICENCA where ID_CLIENTEINTERNO = " + nidCliente + " order by DT_VALIDADE desc");
                if (qLicenca.HasRows)
                {
                    while (qLicenca.Read())
                    {
                        lista.Add(new IN_CHAVELICENCAModel()
                        {
                            ID_CHAVELICENCA = Convert.ToInt64(qLicenca["ID_CHAVELICENCA"]),
                            ID_CLIENTEINTERNO = nidCliente,
                            DS_CHAVE = (string)qLicenca["DS_CHAVE"],
                            DT_VALIDADE = Convert.ToDateTime(qLicenca["DT_VALIDADE"])
                        });
                    }
                }

                return lista;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (qLicenca != null)
                    if (!qLicenca.IsClosed) qLicenca.Close();

                con.FechaConexao();
            }
        }

        [HttpGet]
        [Route("CriarLicenca")]
        public void CriarLicenca(long nidCliente)
        {
            var con = new Conexao();
            SqlDataReader qPlano = null;
            SqlDataReader qCliente = null;
            try
            {
                qPlano = con.ExecQuery("select * from solari.CM_PLANO where ID_PLANO in(select ID_PLANO from solari.CM_CLIENTEINTERNO where ID_CLIENTEINTERNO = " + nidCliente + ")");
                if (qPlano.HasRows)
                {
                    qPlano.Read();
                    var chave = Guid.NewGuid().ToString();

                    qCliente = con.ExecQuery("select * from solari.CM_CLIENTEINTERNO where ID_CLIENTEINTERNO = " + nidCliente);
                    qCliente.Read();

                    var dataValidade = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, qCliente["NR_DIAVENCIMENTO"] == DBNull.Value ? DateTime.Now.Date.Day : Convert.ToInt32(qCliente["NR_DIAVENCIMENTO"]));
                    if (qPlano["FL_PLANO"].ToString() == "M") dataValidade = dataValidade.AddMonths(1);
                    if (qPlano["FL_PLANO"].ToString() == "T") dataValidade = dataValidade.AddMonths(3);
                    if (qPlano["FL_PLANO"].ToString() == "S") dataValidade = dataValidade.AddMonths(6);
                    if (qPlano["FL_PLANO"].ToString() == "A") dataValidade = dataValidade.AddYears(1);

                    dataValidade = dataValidade.AddDays(7);

                    con.ExecCommand("insert into solari.IN_CHAVELICENCA values ("+nidCliente+",'"+chave+"','"+dataValidade.Date.ToString("yyyy-MM-dd") + "')");
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (qPlano != null)
                    if (!qPlano.IsClosed) qPlano.Close();
                if (qCliente != null)
                    if (!qCliente.IsClosed) qCliente.Close();

                con.FechaConexao();
            }
        }
    }
}
