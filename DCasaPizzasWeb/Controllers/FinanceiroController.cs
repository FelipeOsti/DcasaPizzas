﻿using DCasaPizzasWeb.Models;
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

        internal void GerarFinanceiroImplantacao(IN_CLIENTEINTERNOModel cli)
        {
            var con = new Conexao();
            try
            {
                var ddtPriParcela = new DateTime(DateTime.Now.Year, DateTime.Now.Month, cli.NR_DIAVENCIMENTO);
                if (ddtPriParcela.Date < DateTime.Now.Date) ddtPriParcela = ddtPriParcela.AddMonths(1);

                var retorno = CriarDocumento(new DocumentoFinModel()
                {
                    sdsDocum = "Implantação PDV",
                    FL_TIPODOCUM = "I",
                    nidCliente = cli.ID_CLIENTEINTERNO,
                    nrParcelas = cli.NR_PARCELASIMPLANT,
                    vlDesconto = 0,
                    vlDocum = cli.VL_IMPLANTACAO,
                    ddtPriParcela = ddtPriParcela
                });
                if (retorno != "OK") throw new Exception(retorno);
            }
            catch
            {
                throw;
            }
        }

        internal string GerarFinanceiroMensalidade(long nidCliente)
        {
            var con = new Conexao();
            SqlDataReader qContas = null;
            SqlDataReader qPlano = null;
            try
            {

                qPlano = con.ExecQuery("select * from solari.CM_PLANO where ID_PLANO in(select ID_PLANO from solari.CM_CLIENTEINTERNO where ID_CLIENTEINTERNO = " + nidCliente + ")");
                if (!qPlano.HasRows) throw new Exception("Cliente sem nenhum plano relacionado!");

                var dataAtual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var ddtIni = dataAtual.Year + "-" + dataAtual.Month + "-1";

                qPlano.Read();
                if (qPlano["FL_PLANO"].ToString() == "M") dataAtual = dataAtual.AddMonths(1);
                if (qPlano["FL_PLANO"].ToString() == "T") dataAtual = dataAtual.AddMonths(3);
                if (qPlano["FL_PLANO"].ToString() == "S") dataAtual = dataAtual.AddMonths(6);
                if (qPlano["FL_PLANO"].ToString() == "A") dataAtual = dataAtual.AddYears(1);

                dataAtual = dataAtual.AddDays(-1);
                var ddtFim = dataAtual.Year + "-" + dataAtual.Month + "-" + dataAtual.Day;

                qContas = con.ExecQuery("select * from solari.CR_PARCELA where ID_DOCUM in(select ID_DOCUM from solari.CR_DOCUM where ID_CLIENTEINTERNO = " + nidCliente + " and FL_TIPODOCUM = 'M') and DT_VENCIMENTO between '" + ddtIni + "' and '" + ddtFim + "'");
                if (qContas.HasRows) return "";

                var retorno = CriarDocumento(new DocumentoFinModel()
                {
                    sdsDocum = "Mensalidade PDV",
                    FL_TIPODOCUM = "M",
                    nidCliente = nidCliente,
                    nrParcelas = 1,
                    vlDesconto = 0,
                    vlDocum = Convert.ToDouble(qPlano["VL_PLANO"]),
                    ddtPriParcela = DateTime.Now.Date.AddDays(3)
                });

                if (retorno != "OK") throw new Exception(retorno);

                return "";
            }
            catch (Exception e)
            {
                return e.Message;
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
                con.ExecCommand("insert into solari.CR_DOCUM values('" + docum.sdsDocum + "'," + docum.nidCliente + "," + docum.vlDocum.ToString(nfi) + ",GETDATE()," + docum.vlDesconto.ToString(nfi) + ",'"+docum.FL_TIPODOCUM+"')");

                qDocto = con.ExecQuery("select max(ID_DOCUM) as ID_DOCUM from solari.CR_DOCUM where DS_DOCUM = '" + docum.sdsDocum + "' and ID_CLIENTEINTERNO = " + docum.nidCliente);
                qDocto.Read();
                var nidDocto = Convert.ToInt64(qDocto["ID_DOCUM"]);

                if (docum.nrParcelas <= 0) docum.nrParcelas = 1;
                double nvlParcela = (docum.vlDocum - docum.vlDesconto) / docum.nrParcelas;

                DateTime ddtVencimento = docum.ddtPriParcela;
                if (ddtVencimento.Date <= DateTime.Now.Date) ddtVencimento = DateTime.Now.Date.AddDays(1);

                for (int i = 1; i <= docum.nrParcelas;i++ )
                {          
                    con.ExecCommand("insert into solari.CR_PARCELA values(" + nidDocto + "," + nvlParcela.ToString(nfi) + ",0,'" + ddtVencimento.Date.ToString("yyyy-MM-dd") + "')");
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
