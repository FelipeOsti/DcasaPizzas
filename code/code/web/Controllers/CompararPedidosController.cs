using FluentDateTime;
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
    //[Authorize]
    [RoutePrefix("api/CompararPedidos")]
    public class CompararPedidosController : ApiController
    {
        string sdsFiltro = "";

        [Route("BuscarDadosGraficos")]
        [HttpGet]
        public Grafico BuscarDadosGraficos(string sdsParam, int dia, int mes, int ano)
        {
            if (sdsParam == "Industrial")
                sdsFiltro = GraficoDadosController.sdsFiltroIndustrial;
            else if (sdsParam == "Distribuicao")
                sdsFiltro = GraficoDadosController.sdsFiltroDistribuicao;
            else if (sdsParam == "Onix")
                sdsFiltro = GraficoDadosController.sdsFiltroOnix.Replace(")) and ", "))");
            else if (sdsParam == "Consolidado")
                sdsFiltro = "(" + GraficoDadosController.sdsFiltroIndustrial + " or " + GraficoDadosController.sdsFiltroDistribuicao + " or " + GraficoDadosController.sdsFiltroOnix.Replace(")) and ", "))") + ")";

            List<double> lstCarteira = new List<double>();
            List<double> lstPedidos = new List<double>();

            var lstLegenda = new List<string>();

            dia = dia - 6;

            for (int x = dia; x <= dia + 6; x++)
            {
                lstCarteira.Add(GetComposicaoCarteira(new DateTime(ano, mes, x),true).Count());
                lstPedidos.Add(GetComposicaoPedidos(new DateTime(ano, mes, x),false).Count());

                lstLegenda.Add(new DateTime(ano, mes, x).ToString("ddd"));
            }

            Grafico graf = new Grafico()
            {
                ID_MENUAPP = 0,
                sdsTitulo = "Comparar Pedido x Carteira",
                sdsTipoGrafico = 2 //Linha
            };

            graf.Dados = new List<DadosGrafico>();

            graf.Dados.Add(new DadosGrafico()
            {
                Entries = new Models.Entry() { Value = lstPedidos },
                Label = "Pedidos Emitidos",
                cor = new CorGraf(54, 56, 217, 200)
            });
            graf.Dados.Add(new DadosGrafico()
            {
                Entries = new Models.Entry() { Value = lstCarteira },
                Label = "Carteira",
                cor = new CorGraf(0, 160, 50, 200)
            });

            graf.Legendas = lstLegenda;

            return graf;
        }

        [Route("GetQtdePedidos")]
        [HttpGet]
        public double GetQtdePedidos(string sdsParam, int dia, int mes, int ano)
        {
            if (sdsParam == "Industrial")
                sdsFiltro = GraficoDadosController.sdsFiltroIndustrial;
            else if (sdsParam == "Distribuicao")
                sdsFiltro = GraficoDadosController.sdsFiltroDistribuicao;
            else if (sdsParam == "Onix")
                sdsFiltro = GraficoDadosController.sdsFiltroOnix.Replace(")) and ", "))");
            else if (sdsParam == "Consolidado")
                sdsFiltro = "(" + GraficoDadosController.sdsFiltroIndustrial + " or " + GraficoDadosController.sdsFiltroDistribuicao + " or " + GraficoDadosController.sdsFiltroOnix.Replace(")) and ", "))") + ")";
            return GetComposicaoPedidos(new DateTime(ano, mes, dia),false).Count();
        }

        [Route("GetQtdeCarteira")]
        [HttpGet]
        public double GetQtdeCarteira(string sdsParam, int dia, int mes, int ano)
        {
            if (sdsParam == "Industrial")
                sdsFiltro = GraficoDadosController.sdsFiltroIndustrial;
            else if (sdsParam == "Distribuicao")
                sdsFiltro = GraficoDadosController.sdsFiltroDistribuicao;
            else if (sdsParam == "Onix")
                sdsFiltro = GraficoDadosController.sdsFiltroOnix.Replace(")) and ", "))");
            else if (sdsParam == "Consolidado")
                sdsFiltro = "(" + GraficoDadosController.sdsFiltroIndustrial + " or " + GraficoDadosController.sdsFiltroDistribuicao + " or " + GraficoDadosController.sdsFiltroOnix.Replace(")) and ", "))") + ")";
            return GetComposicaoCarteira(new DateTime(ano, mes, dia),true).Count();
        }

        [Route("GetComparaPedidos")]
        [HttpGet]
        public List<modelPedidos> GetComparaPedidos(string sdsParam, int dia, int mes, int ano, int diaC, int mesC, int anoC)
        {
            if (sdsParam == "Industrial")
                sdsFiltro = GraficoDadosController.sdsFiltroIndustrial;
            else if (sdsParam == "Distribuicao")
                sdsFiltro = GraficoDadosController.sdsFiltroDistribuicao;
            else if (sdsParam == "Onix")
                sdsFiltro = GraficoDadosController.sdsFiltroOnix.Replace(")) and ", "))");
            else if (sdsParam == "Consolidado")
                sdsFiltro = "(" + GraficoDadosController.sdsFiltroIndustrial + " or " + GraficoDadosController.sdsFiltroDistribuicao + " or " + GraficoDadosController.sdsFiltroOnix.Replace(")) and ", "))") + ")";
            try
            {
                List<modelPedidos> lstRetorno = new List<modelPedidos>();

                var lstPedidos = GetComposicaoPedidos(new DateTime(ano, mes, dia),false);
                var lstPedidosC = GetComposicaoPedidos(new DateTime(anoC, mesC, diaC),false);

                foreach (double NrPedido in lstPedidos)
                {
                    bool bboExiste = false;

                    foreach(double NrPedidoC in lstPedidosC)
                    {
                        if(NrPedido == NrPedidoC)
                        {
                            bboExiste = true;
                            break;
                        }
                    }

                    if (!bboExiste)
                    {
                        lstRetorno.Add(RecuperaPedido(NrPedido,new DateTime(ano,mes,dia),"Saiu"));
                    }
                }

                foreach (double NrPedidoC in lstPedidosC)
                {
                    bool bboExiste = false;

                    foreach (double NrPedido in lstPedidos)
                    {
                        if (NrPedido == NrPedidoC)
                        {
                            bboExiste = true;
                            break;
                        }
                    }

                    if (!bboExiste)
                    {
                        lstRetorno.Add(RecuperaPedido(NrPedidoC, new DateTime(ano, mes, dia), "Entrou"));
                    }
                }

                return lstRetorno;
            }
            catch
            {
                throw;
            }
        }

        [Route("GetComparaCarteira")]
        [HttpGet]
        public List<modelPedidos> GetComparaCarteira(String sdsParam, int dia, int mes, int ano, int diaC, int mesC, int anoC)
        {
            if (sdsParam == "Industrial")
                sdsFiltro = GraficoDadosController.sdsFiltroIndustrial;
            else if (sdsParam == "Distribuicao")
                sdsFiltro = GraficoDadosController.sdsFiltroDistribuicao;
            else if (sdsParam == "Onix")
                sdsFiltro = GraficoDadosController.sdsFiltroOnix.Replace(")) and ", "))");
            else if (sdsParam == "Consolidado")
                sdsFiltro = "(" + GraficoDadosController.sdsFiltroIndustrial + " or " + GraficoDadosController.sdsFiltroDistribuicao + " or " + GraficoDadosController.sdsFiltroOnix.Replace(")) and ", "))") + ")";
            try
            {
                List<modelPedidos> lstRetorno = new List<modelPedidos>();

                var lstPedidos = GetComposicaoCarteira(new DateTime(ano, mes, dia),true);
                var lstPedidosC = GetComposicaoCarteira(new DateTime(anoC, mesC, diaC),true);

                foreach (double NrPedido in lstPedidos)
                {
                    bool bboExiste = false;

                    foreach (double NrPedidoC in lstPedidosC)
                    {
                        if (NrPedido == NrPedidoC)
                        {
                            bboExiste = true;
                        }
                    }

                    if (!bboExiste && NrPedido > 0)
                    {
                        var ped = RecuperaPedido(NrPedido, new DateTime(ano, mes, dia), "Entrou");
                        if (ped.nrPedido > 0)
                            lstRetorno.Add(ped);                        
                    }
                }

                foreach (double NrPedidoC in lstPedidosC)
                {
                    bool bboExiste = false;

                    foreach (double NrPedido in lstPedidos)
                    {
                        if (NrPedido == NrPedidoC)
                        {
                            bboExiste = true;
                        }
                    }

                    if (!bboExiste && NrPedidoC > 0)
                    {
                        var ped = RecuperaPedido(NrPedidoC, new DateTime(ano, mes, dia), "Saiu");
                        if(ped.nrPedido > 0)
                            lstRetorno.Add(ped);
                    }
                }

                return lstRetorno;
            }
            catch
            {
                throw;
            }
        }

        private modelPedidos RecuperaPedido(double nrPedido, DateTime data, string sdsStatus)
        {
            DbDataReader dataP = null;
            var con = Conexao.Instance(null);
            try
            {
                string dataIni = "1/" + data.Month + "/" + data.Year;
                data = data.LastDayOfMonth();
                string dataFim = data.Day + "/" + data.Month + "/" + data.Year;

                var retorno = new modelPedidos();

                string sdsSql = @"select
  distinct a.NR_PEDIDO,b.NM_RAZAO,a.VL_TOTPED,c.DT_FATURA,a.CD_SITDOC,
  (select OB_REPROG||' - '||(select DS_TPREP from PE_TPREP where PE_TPREP.CD_TPREP = e.CD_TPREP) from PE_LGREP e where 
        e.CD_ESTAB = a.CD_ESTAB and e.CD_ESTGER = a.CD_ESTGER and e.NR_PEDIDO = a.NR_PEDIDO and rownum = 1) as DS_MOTIVO,
  g.DS_CLASSIF
from PE_PEDID a, CM_CLIEN b, PE_RITDE c, PE_CLASPED f, PE_CLASSIF g where a.NR_PEDIDO = " + nrPedido + @" and 
a.CD_CLIENTE = b.CD_CLIENTE and
c.CD_ESTAB = a.CD_ESTAB and
c.CD_ESTGER = a.CD_ESTGER and
c.NR_PEDIDO = a.NR_PEDIDO and
f.CD_ESTAB = a.CD_ESTAB and
f.CD_ESTGER = a.CD_ESTGER and
f.NR_PEDIDO = a.NR_PEDIDO and
g.ID_CLASSIF = f.ID_CLASSIF and
g.CD_ESTAB = a.CD_ESTABFAT and
c.DT_FATURA = (select max(d.DT_FATURA) from PE_RITDE d where
                a.CD_ESTAB = d.CD_ESTAB and a.CD_ESTGER = d.CD_ESTGER and
                a.NR_PEDIDO = d.NR_PEDIDO";

                if (sdsStatus == "Entrou") sdsSql = sdsSql + " and d.DT_FATURA between to_date('" + dataIni + "', 'dd/mm/yyyy') and to_date('" + dataFim + "','dd/mm/yyyy'))";
                if (sdsStatus == "Saiu") sdsSql = sdsSql + ")";

                dataP = con.execQuery(sdsSql);

                if (dataP.HasRows)
                {
                    dataP.Read();

                    retorno.nrPedido = Convert.ToInt64(dataP.GetValue(dataP.GetOrdinal("NR_PEDIDO")));
                    retorno.dsClassif = dataP.GetString(dataP.GetOrdinal("DS_CLASSIF"));
                    retorno.vlPedido = Convert.ToInt64(dataP.GetInt64(dataP.GetOrdinal("VL_TOTPED")));
                    retorno.dtFatura = dataP.GetDateTime(dataP.GetOrdinal("DT_FATURA"));
                    if(!dataP.IsDBNull(dataP.GetOrdinal("DS_MOTIVO")))
                        retorno.dsMotivo = dataP.GetString(dataP.GetOrdinal("DS_MOTIVO"));
                    retorno.dsCliente = dataP.GetString(dataP.GetOrdinal("NM_RAZAO"));
                    retorno.dsSituacao = sdsStatus;

                    if(sdsStatus == "Saiu" && Convert.ToInt32(dataP.GetValue(dataP.GetOrdinal("CD_SITDOC"))) == 6)
                        retorno.dsMotivo = "Pedido Cancelado";
                    else if (sdsStatus == "Saiu")
                        retorno.dsMotivo = "Prorrogação [ "+retorno.dsMotivo +" ]";
                    if (sdsStatus == "Entrou" && (retorno.dsMotivo == "" || retorno.dsMotivo == null))
                        retorno.dsMotivo = "Novo Pedido";
                    else if (sdsStatus == "Entrou" && retorno.dsMotivo != "")
                        retorno.dsMotivo = "Antecipação - [ "+retorno.dsMotivo+" ]";
                }

                return retorno;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dataP != null)
                    if (!dataP.IsClosed) dataP.Close();
                con.fechaCon();
            }
        }

        private List<double> GetComposicaoPedidos(DateTime dia, bool ConsideraCancelado)
        {
            string dataInicio = "1/" + dia.Month + "/" + dia.Year;
            string dataFim = dia.Day + 1 + "/" + dia.Month + "/" + dia.Year;

            DbDataReader dataR = null;

            var retorno = new List<double>();

            var con = Conexao.Instance(null);

            try
            {
                string sdsSitdoc = "";
                if (ConsideraCancelado)
                    sdsSitdoc = "2,4,7,8,14";
                else
                    sdsSitdoc = "2,4,6,7,8,14";

                string sdsSql = @"select distinct CD_ESTAB,CD_ESTGER,NR_PEDIDO from PE_PEDID where "+ sdsFiltro + @" and
DT_EMISPED >= to_date('" + dataInicio + @"','dd/mm/yyyy') and DT_EMISPED < to_date('" + dataFim + @"','dd/mm/yyyy')
and CD_SITDOC not in("+ sdsSitdoc + @") and (CD_NATOP, CD_NATOPIT) in(select CD_NATOP, CD_NATOPIT from CT_ITNAT where BO_CARTEIR = 'T')";
                if (ConsideraCancelado)
                    sdsSql = sdsSql + " and (select count(*) from PE_LOGSITPED where CD_ESTAB = PE_PEDID.CD_ESTAB and CD_ESTGER = PE_PEDID.CD_ESTGER and NR_PEDIDO = PE_PEDID.NR_PEDIDO and CD_SITDOC = 6 and DT_ALTERACAO < to_date('" + dataFim + @"','dd/mm/yyyy')) = 0 ";

                dataR = con.execQuery(sdsSql);
                if (dataR.HasRows)
                {
                    while (dataR.Read())
                        retorno.Add(Convert.ToInt64(dataR.GetValue(dataR.GetOrdinal("NR_PEDIDO"))));
                }

                return retorno;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dataR != null)
                    if (!dataR.IsClosed) dataR.Close();
                con.fechaCon();
            }
        }

        private List<double> GetComposicaoCarteira(DateTime dia, bool ConsideraCancelado)
        {
            string dataInicio = "1/" + dia.Month + "/" + dia.Year;
            string dataAtual = dia.Day + "/" + dia.Month + "/" + dia.Year;
            string dataAtual1 = dia.Day + 1 + "/" + dia.Month + "/" + dia.Year;
            dia = dia.LastDayOfMonth();
            string dataFim = dia.Day + "/" + dia.Month + "/" + dia.Year;

            DbDataReader dataR = null;

            var retorno = new List<double>();

            var con = Conexao.Instance(null);
            try
            {
                string sdsSitdoc = "";
                if (ConsideraCancelado)
                    sdsSitdoc = "2,4,7,8,14";
                else
                    sdsSitdoc = "2,4,6,7,8,14";

                string sdsSql = @"select distinct CD_ESTAB,CD_ESTGER,NR_PEDIDO from PE_RITDE a where a.CD_MERCADO = 200 and " + sdsFiltro + @" and
(a.CD_ESTAB, a.CD_ESTGER, a.NR_PEDIDO) in(select CD_ESTAB, CD_ESTGER, NR_PEDIDO from PE_PEDID where " + sdsFiltro + @" and
CD_SITDOC not in(" + sdsSitdoc + @") and (CD_NATOP, CD_NATOPIT) in(select CD_NATOP, CD_NATOPIT from CT_ITNAT where BO_CARTEIR = 'T') and 
DT_EMISPED >= to_date('" + dataInicio + @"','dd/mm/yyyy') and DT_EMISPED < to_date('" + dataAtual1 + @"','dd/mm/yyyy')) and
a.DT_FATURA between to_date('" + dataInicio + @"','dd/mm/yyyy') and to_date('" + dataFim + @"','dd/mm/yyyy') and (DT_REPROG is null or DT_REPROG > to_date('" + dataAtual + @"','dd/mm/yyyy')) and
(CASE when a.NR_SEQRITD > 1 then
(select max(DT_REPROG) as DATA from PE_RITDE b where a.CD_ESTAB = b.CD_ESTAB and a.CD_ESTGER = b.CD_ESTGER and a.NR_PEDIDO = b.NR_PEDIDO and
   a.NR_SEQPED = b.NR_SEQPED and b.NR_SEQRITD = a.NR_SEQRITD - 1) else to_date('" + dataAtual + @"','dd/mm/yyyy') end) <= to_date('" + dataAtual + @"','dd/mm/yyyy')";
                if(ConsideraCancelado)
                    sdsSql = sdsSql + " and (select count(*) from PE_LOGSITPED where CD_ESTAB = a.CD_ESTAB and CD_ESTGER = a.CD_ESTGER and NR_PEDIDO = a.NR_PEDIDO and CD_SITDOC = 6 and DT_ALTERACAO < to_date('" + dataAtual1 + @"','dd/mm/yyyy')) = 0 ";

                dataR = con.execQuery(sdsSql);
                if (dataR.HasRows)
                {                    
                    while (dataR.Read())
                        retorno.Add(Convert.ToInt64(dataR.GetValue(dataR.GetOrdinal("NR_PEDIDO"))));
                }

                return retorno;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dataR != null)
                    if (!dataR.IsClosed) dataR.Close();
                con.fechaCon();
            }
        }
    }
}