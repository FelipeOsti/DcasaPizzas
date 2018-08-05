using Newtonsoft.Json;
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
    [RoutePrefix("API/planfin")]
    public class PlanilhaFinanceiraController : ApiController
    {
        List<PlanilhaFinanceira> listaPlanilha = new List<PlanilhaFinanceira>();

        [HttpGet]
        [Route("getPlanilhas")]
        public List<PlanilhaFinanceira> getPlanilhas(String sdsEmail)
        {
            DbDataReader qryPlanPar = null;
            Conexao con = Conexao.Instance(sdsEmail);
            try
            {
                var usuar = UsuarioEmailController.getUsuario(sdsEmail);
                TarefaUNIFACE tarefa = new TarefaUNIFACE()
                {
                    DS_EMAIL = usuar.DS_EMAIL,
                    CD_SISTEMA = "PLANILHA",
                    CD_CHAVE = "CD_USUAR="+ usuar.CD_USUARIO,
                    CD_TAREFA = "CONSULTA"
                };

                TarefaUNIFACE tarefaRetorno = new TarefaUNIFACE();
                UnifaceController uniface = new UnifaceController();
                tarefaRetorno = uniface.startSIGE(tarefa);

                if (tarefaRetorno.FL_STATUS == 4 && tarefaRetorno.DS_RETORNO != "" && tarefaRetorno.DS_RETORNO != null)
                {
                    List<PlanilhaFinanceira> lstPlanilha = new List<PlanilhaFinanceira>();

                    lstPlanilha = JsonConvert.DeserializeObject<List<PlanilhaFinanceira>>(tarefaRetorno.DS_RETORNO);

                    foreach (var plan in lstPlanilha)
                    {
                        if (plan.NR_PLANILHA > 0)
                        {
                            long nidPlanilha = plan.NR_PLANILHA;
                            double nvlValor = plan.VL_VALOR;
                            string sdsCliente = plan.DS_CLIENTE;
                            string sdtPlan = plan.DT_PLANPFC;
                            int nnrAtraso = plan.NR_ATRASO;

                            var nvlPedido = buscaValorPedidoPendente(nidPlanilha);

                            listaPlanilha.Add(new PlanilhaFinanceira { NR_PLANILHA = nidPlanilha, DS_CLIENTE = sdsCliente, VL_VALOR = nvlValor, DT_PLANPFC = sdtPlan, NR_ATRASO = nnrAtraso, FL_TIPOPLAN = "0", VL_PEDIDO = nvlPedido });
                        }
                    }
                }               

                string sdsSQL = @"select a.ID_PLANPFC,
(select sum(VL_TOTAL) from VCD_PLANPFC d where d.ID_PLANPFC = a.ID_PLANPFC) as VL_TOTAL,a.ID_NIVEL,
c.CD_CLIENTE,c.NM_RAZAO,to_char(b.DT_PLANPFC,'dd/mm/yyyy hh24:mi:ss') as DT_PLANPFC, b.NR_ATRASO
from CD_PLANPAR a, CD_PLANPFC b, CM_CLIEN c where
a.ID_PLANPFC = b.ID_PLANPFC and
c.CD_CLIENTE = b.CD_CLIENTE and
a.CD_USUAR is null and a.FL_TPLIBER is null and a.OB_PARECER is null and a.DT_PARECER is null and a.ID_NIVEL in (
  select id_nivel from CD_USUNIVEL where CD_USUAR = '" + UsuarioEmailController.getUsuario(sdsEmail).CD_USUARIO + @"' and DT_INATIVO is null and ID_NIVEL in (
  select id_nivel from CD_NIVEL where CD_MERCADO in (200, 300)))";

                qryPlanPar = con.execQuery(sdsSQL);

                if (qryPlanPar.HasRows)
                {
                    while (qryPlanPar.Read())
                    {
                        long nidPlanilha = Convert.ToInt64(qryPlanPar.GetValue(qryPlanPar.GetOrdinal("ID_PLANPFC")));
                        double nvlValor = Convert.ToDouble(qryPlanPar.GetValue(qryPlanPar.GetOrdinal("VL_TOTAL")));
                        string sdsCliente = qryPlanPar.GetString(qryPlanPar.GetOrdinal("NM_RAZAO"));
                        string sdtPlan = qryPlanPar.GetString(qryPlanPar.GetOrdinal("DT_PLANPFC"));
                        int nnrAtraso = Convert.ToInt32(qryPlanPar.GetValue(qryPlanPar.GetOrdinal("NR_ATRASO")));
                        int nidNivel = Convert.ToInt32(qryPlanPar.GetValue(qryPlanPar.GetOrdinal("ID_NIVEL")));
                        long ncdCliente = Convert.ToInt64(qryPlanPar.GetValue(qryPlanPar.GetOrdinal("CD_CLIENTE")));

                        var bboExiste = false;
                        foreach (PlanilhaFinanceira plan in listaPlanilha)
                        {
                            if (plan.NR_PLANILHA == nidPlanilha)
                            {
                                bboExiste = true;
                                break;
                            }
                        }

                        if (!bboExiste)
                        {
                            var nvlPedido = buscaValorPedidoPendente(nidPlanilha);
                            listaPlanilha.Add(new PlanilhaFinanceira { NR_PLANILHA = nidPlanilha, DS_CLIENTE = sdsCliente, VL_VALOR = nvlValor, DT_PLANPFC = sdtPlan, NR_ATRASO = nnrAtraso, FL_TIPOPLAN = "1", ID_NIVEL = nidNivel, CD_CLIENTE = ncdCliente, DS_EMAIL = sdsEmail, VL_PEDIDO = nvlPedido });
                        }
                    }
                }
                
                return listaPlanilha;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (qryPlanPar != null)
                    if (!qryPlanPar.IsClosed) qryPlanPar.Close();
                con.fechaCon();
            }
        }

        [HttpPost]
        [Route("LiberaReprovaPlanilha")]
        public string LiberaReprovaPlanilha(PlanilhaFinanceira planilha, string sflLiberReprova, string sdsEmail)
        {
            try
            {
                string sdsTarefa = "APROVA";
                if (sflLiberReprova == "R") sdsTarefa = "REPROVA";

                string sdsSistema = "PLANILHA";
                string sdsChave = planilha.NR_PLANILHA.ToString() + "+;" + planilha.DS_PARECER;
                if (planilha.FL_TIPOPLAN == "1")
                {
                    sdsChave = planilha.NR_PLANILHA.ToString() + "+;" + planilha.DS_PARECER + "+;" + planilha.DS_CLIENTE + "+;" + planilha.CD_CLIENTE + "+;" + planilha.ID_NIVEL;
                    sdsSistema = "PLANILHA1";
                }

                TarefaUNIFACE tarefa = new TarefaUNIFACE()
                {
                    CD_SISTEMA = sdsSistema,
                    CD_TAREFA = sdsTarefa,
                    CD_CHAVE = sdsChave,
                    DS_EMAIL = UsuarioEmailController.getUsuario(sdsEmail).DS_EMAIL
            };

                var novaTarefa = new TarefaUNIFACE();
                UnifaceController uniface = new UnifaceController();
                novaTarefa = uniface.startSIGE(tarefa);
                if (novaTarefa.FL_STATUS != 4)
                    return novaTarefa.DS_RETORNO;
                else
                    return "";
            }
            catch
            {
                throw;
            }

        }

        private double buscaValorPedidoPendente(long nidPlanilha)
        {
            DbDataReader qryPlanPed = null;
            Conexao con = Conexao.Instance("");
            try
            {
                qryPlanPed = con.execQuery("select sum(b.VL_TOTPED) as VL_PEDIDO from CD_PLANPED a, PE_PEDID b where a.ID_PLANPFC = " + nidPlanilha + " and a.FL_SITPED = 'A' and " +
                                           " b.CD_ESTAB = a.CD_ESTAB and b.CD_ESTGER = a.CD_ESTGER and a.NR_PEDIDO = b.NR_PEDIDO");

                if (qryPlanPed.HasRows)
                {
                    return Convert.ToInt64(qryPlanPed.GetValue(qryPlanPed.GetOrdinal("VL_PEDIDO")));
                }

                return 0;
            }
            catch { throw; }
            finally
            {
                if(qryPlanPed != null)
                    if (!qryPlanPed.IsClosed) qryPlanPed.Close();
                con.fechaCon();
            }
        }

    }
}
