using DCasaPizzasWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DCasaPizzasWeb.Controllers
{
    [RoutePrefix("api/financeiro")]
    public class FinanceiroController : ApiController
    {
        [Route("RealizarPagamentoPagSeguro")]
        public void RealizarPagamentoPagSeguro(long nidParcela)
        {
            var con = new Conexao();
            SqlDataReader parcela = null;
            try
            {
                parcela = con.ExecQuery("select * from solari.cr_parcela where ID_PARCELA = " + nidParcela);
                if(parcela.HasRows)
                {
                    parcela.Read();
                    CriarLiquidacao(nidParcela, Convert.ToDouble(parcela["VL_PARCELA"]));
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (parcela != null)
                    if (!parcela.IsClosed) parcela.Close();
                con.FechaConexao();
            }
        }

        [HttpGet]
        [Route("GetDocumentos/{nidCliente}/{sflPago}")]
        public List<CR_DOCUMModel> GetDocumentos(long nidCLiente, string sflPago)
        {
            if (string.IsNullOrEmpty(sflPago)) sflPago = "F";

            var con = new Conexao();
            SqlDataReader qDocum = null;
            SqlDataReader qParcela = null;
            try
            {
                List<CR_DOCUMModel> retorno = new List<CR_DOCUMModel>();

                var sdsSinal = sflPago == "F" ? ">" : "<=";
                qDocum = con.ExecQuery("select * from solari.CR_DOCUM where ID_CLIENTEINTERNO = " + nidCLiente + " and VL_DOCUM-VL_DESCONTO " + sdsSinal + " (select sum(VL_PAGO) from solari.CR_PARCELA where ID_DOCUM = solari.CR_DOCUM.ID_DOCUM)");

                if (qDocum.HasRows)
                {
                    while (qDocum.Read())
                    {
                        retorno.Add(new CR_DOCUMModel()
                        {
                            DS_DOCUM = qDocum["DS_DOCUM"] == DBNull.Value ? "" : (string)qDocum["DS_DOCUM"],
                            ID_DOCUM = qDocum["ID_DOCUM"] == DBNull.Value ? 0 : Convert.ToInt64(qDocum["ID_DOCUM"]),
                            VL_DOCUM = qDocum["VL_DOCUM"] == DBNull.Value ? 0 : Convert.ToDouble(qDocum["VL_DOCUM"]),
                            VL_DESCONTO = qDocum["VL_DESCONTO"] == DBNull.Value ? 0 : Convert.ToDouble(qDocum["VL_DESCONTO"]),
                            parcelas = new List<CR_PARCELAModel>()                            
                        });

                        if (qParcela != null)
                        {
                            if (!qParcela.IsClosed)
                            {
                                qParcela.Close();
                                con.FechaUltimaConexao();
                            }
                        }

                        qParcela = con.ExecQuery("select * from solari.CR_PARCELA where ID_DOCUM = " + Convert.ToInt64(qDocum["ID_DOCUM"]));
                        if (qParcela.HasRows)
                        {
                            while (qParcela.Read())
                            {
                                retorno[retorno.Count - 1].parcelas.Add(new CR_PARCELAModel()
                                {
                                    DT_VENCIMENTO = qParcela["DT_VENCIMENTO"] == DBNull.Value ? null : Convert.ToDateTime(qParcela["DT_VENCIMENTO"]).ToString("dd'/'MM'/'yyyy"),
                                    ID_PARCELA = qParcela["ID_PARCELA"] == DBNull.Value ? 0 : Convert.ToInt64(qParcela["ID_PARCELA"]),
                                    VL_PAGO = qParcela["VL_PAGO"] == DBNull.Value ? 0 : Convert.ToDouble(qParcela["VL_PAGO"]),
                                    VL_PARCELA = qParcela["VL_PARCELA"] == DBNull.Value ? 0 : Convert.ToDouble(qParcela["VL_PARCELA"])
                                });
                            }
                        }
                    }
                }

                return retorno;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (qDocum != null)
                    if (!qDocum.IsClosed) qDocum.Close();

                if (qParcela != null)
                    if (!qParcela.IsClosed) qParcela.Close();

                con.FechaConexao();
            }
        }

        [HttpPost]
        [Route("CriarDocumento")]
        public string CriarDocumento(DocumentoFinModel docum)
        {
            var con = new Conexao();
            SqlDataReader qDocto = null;
            try
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                con.ExecCommand("insert into solari.CR_DOCUM values('" + docum.sdsDocum + "'," + docum.nidCliente + "," + docum.vlDocum.ToString(nfi) + ",GETDATE()," + docum.vlDesconto.ToString(nfi) + ")");

                qDocto = con.ExecQuery("select max(ID_DOCUM) as ID_DOCUM from solari.CR_DOCUM where DS_DOCUM = '" + docum.sdsDocum + "' and ID_CLIENTEINTERNO = " + docum.nidCliente);
                qDocto.Read();
                var nidDocto = Convert.ToInt64(qDocto["ID_DOCUM"]);

                if (docum.nrParcelas <= 0) docum.nrParcelas = 1;
                double nvlParcela = (docum.vlDocum - docum.vlDesconto) / docum.nrParcelas;

                DateTime ddtVencimento = docum.ddtPriParcela;
                if (ddtVencimento.Date <= DateTime.Now.Date) ddtVencimento = DateTime.Now.Date.AddDays(1);

                for (int i = 1; i <= docum.nrParcelas;i++ )
                {          
                    con.ExecCommand("insert into solari.CR_PARCELA values(" + nidDocto + "," + nvlParcela.ToString(nfi) + ",null,'" + ddtVencimento.Date.ToString("yyyy-MM-dd") + "')");
                    ddtVencimento = ddtVencimento.AddMonths(1);
                }

                return "OK";
            }
            catch
            {
                throw;
            }
            finally
            {
                con.FechaConexao();
            }
        }

        private void CriarLiquidacao(long nidParcela, double valor)
        {
            var con = new Conexao();
            SqlDataReader qDoc = null; 
            try
            {
                qDoc = con.ExecQuery("select * from solari.cr_docum where ID_DOCUM in(select ID_DOCUM from solari.CR_PARCELA where ID_PARCELA = " + nidParcela + ")");
                qDoc.Read();
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                con.ExecCommand("insert into solari.CR_LIQUIDACAO values (" + nidParcela+","+valor.ToString(nfi) + ",GETDATE())");

                con.ExecCommand("update solari.CR_PARCELA set VL_PAGO = VL_PAGO + " + valor.ToString(nfi)+" where ID_PARCELA = "+nidParcela);

                LicencaController licenca = new LicencaController();
                licenca.CriarLicenca(Convert.ToInt64(qDoc["ID_CLIENTEINTERNO"]));
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (qDoc != null)
                    if (!qDoc.IsClosed) qDoc.Close();
                con.FechaConexao();
                    
            }
        }
    }
}
