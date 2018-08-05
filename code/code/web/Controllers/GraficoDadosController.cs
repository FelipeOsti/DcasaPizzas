using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAppRoma.Models;
using FluentDateTime;
using System.Web.Http.Results;
using System.Drawing;
using AppRomagnole.Models;

namespace WebAppRoma.Controllers
{
    [Authorize]
    [RoutePrefix("api/DadosGrafico")]
    public class GraficoDadosController : ApiController
    {

        #region Definição dos filtros

        DbDataReader dataR = null;
        List<double> retorno = null;

        static string sdsFiltroTransf = " ((CD_ESTAB = 8 and CD_ESTGER = 8) or (CD_ESTAB = 10 and CD_ESTGER = 8) or (CD_ESTAB = 8 and CD_ESTGER = 9) or " +
                                 "  (CD_ESTAB = 42 and CD_ESTGER = 8) or(CD_ESTAB = 43 and CD_ESTGER = 8) or (CD_ESTAB = 45 and CD_ESTGER = 8)) and ";

        static string sdsFiltroFerragens = " ((CD_ESTAB = 2 and CD_ESTGER = 2) or (CD_ESTAB = 34 and CD_ESTGER = 2) or (CD_ESTAB = 35 and CD_ESTGER = 35) or " +
                                    "  (CD_ESTAB = 42 and CD_ESTGER = 2) or (CD_ESTAB = 43 and CD_ESTGER = 2)) and ";

        static string sdsFiltroFerragensNF = " ((CD_ESTAB = 2 and CD_ESTGER = 2) or (CD_ESTAB = 34 and CD_ESTGER = 2) or (CD_ESTAB = 35 and CD_ESTGER = 35) or " +
                                    "  (CD_ESTAB = 42 and CD_ESTGER = 42) or (CD_ESTAB = 43 and CD_ESTGER = 43)) and ";

        public static string sdsFiltroOnix = " ((CD_ESTAB = 85 and CD_ESTGER = 85) or (CD_ESTAB = 86 and CD_ESTGER = 86) or (CD_ESTAB = 87 and CD_ESTGER = 87) or " +
                               "  (CD_ESTAB = 88 and CD_ESTGER = 88) or (CD_ESTAB = 89 and CD_ESTGER = 89)  or (CD_ESTAB = 83 and CD_ESTGER = 83)) and ";

        static string sdsFiltroArtefatos = " ((CD_ESTAB = 13 and CD_ESTGER = 13) or (CD_ESTAB = 15 and CD_ESTGER = 15) or (CD_ESTAB = 37 and CD_ESTGER = 37) or " +
                                    "  (CD_ESTAB = 38 and CD_ESTGER = 38) or (CD_ESTAB = 40 and CD_ESTGER = 40) or (CD_ESTAB = 41 and CD_ESTGER = 41) or " +
                                    "  (CD_ESTAB = 44 and CD_ESTGER = 44) or (CD_ESTAB = 46 and CD_ESTGER = 46)) and ";

        static string sdsConsolidado = " ((CD_ESTAB = 8 and CD_ESTGER = 8) or (CD_ESTAB = 10 and CD_ESTGER = 8) or (CD_ESTAB = 8 and CD_ESTGER = 9) or " +
                                 "  (CD_ESTAB = 42 and CD_ESTGER = 8) or(CD_ESTAB = 43 and CD_ESTGER = 8) or(CD_ESTAB = 45 and CD_ESTGER = 8) or " +
                                 "  (CD_ESTAB = 2 and CD_ESTGER = 2) or (CD_ESTAB = 34 and CD_ESTGER = 2) or (CD_ESTAB = 35 and CD_ESTGER = 35) or " +
                                 "  (CD_ESTAB = 42 and CD_ESTGER = 2) or (CD_ESTAB = 43 and CD_ESTGER = 2) or " +
                                 "  (CD_ESTAB = 42 and CD_ESTGER = 42) or (CD_ESTAB = 43 and CD_ESTGER = 43) or " +
                                 "  (CD_ESTAB = 85 and CD_ESTGER = 85) or (CD_ESTAB = 86 and CD_ESTGER = 86) or (CD_ESTAB = 87 and CD_ESTGER = 87) or " +
                                 "  (CD_ESTAB = 88 and CD_ESTGER = 88) or (CD_ESTAB = 89 and CD_ESTGER = 89) or " +
                                 "  (CD_ESTAB = 13 and CD_ESTGER = 13) or (CD_ESTAB = 15 and CD_ESTGER = 15) or (CD_ESTAB = 37 and CD_ESTGER = 37) or " +
                                 "  (CD_ESTAB = 38 and CD_ESTGER = 38) or (CD_ESTAB = 40 and CD_ESTGER = 40) or (CD_ESTAB = 41 and CD_ESTGER = 41) or " +
                                 "  (CD_ESTAB = 44 and CD_ESTGER = 44) or (CD_ESTAB = 46 and CD_ESTGER = 46)) and ";

        static string sdsConsolidadoMenosOnix = " ((CD_ESTAB = 8 and CD_ESTGER = 8) or (CD_ESTAB = 10 and CD_ESTGER = 8) or (CD_ESTAB = 8 and CD_ESTGER = 9) or " +
                                 "  (CD_ESTAB = 42 and CD_ESTGER = 8) or(CD_ESTAB = 43 and CD_ESTGER = 8) or(CD_ESTAB = 45 and CD_ESTGER = 8) or " +
                                 "  (CD_ESTAB = 2 and CD_ESTGER = 2) or (CD_ESTAB = 34 and CD_ESTGER = 2) or (CD_ESTAB = 35 and CD_ESTGER = 35) or " +
                                 "  (CD_ESTAB = 42 and CD_ESTGER = 2) or (CD_ESTAB = 43 and CD_ESTGER = 2) or " +
                                 "  (CD_ESTAB = 42 and CD_ESTGER = 42) or (CD_ESTAB = 43 and CD_ESTGER = 43) or " +                                 
                                 "  (CD_ESTAB = 13 and CD_ESTGER = 13) or (CD_ESTAB = 15 and CD_ESTGER = 15) or (CD_ESTAB = 37 and CD_ESTGER = 37) or " +
                                 "  (CD_ESTAB = 38 and CD_ESTGER = 38) or (CD_ESTAB = 40 and CD_ESTGER = 40) or (CD_ESTAB = 41 and CD_ESTGER = 41) or " +
                                 "  (CD_ESTAB = 44 and CD_ESTGER = 44) or (CD_ESTAB = 46 and CD_ESTGER = 46)) and ";

        static string sdsClassifDist = "63"; //Transformador Distribuição
        public static string sdsFiltroDistribuicao = "(" + sdsFiltroArtefatos.Replace(")) and", ")) or ") + sdsFiltroFerragens.Replace(")) and", ")) or ") + " (CD_ESTAB,CD_ESTGER,NR_PEDIDO) in (select CD_ESTAB,CD_ESTGER,NR_PEDIDO from PE_CLASPED where ID_CLASSIF in (" + sdsClassifDist + ")))";
        public static string sdsFiltroIndustrial = sdsFiltroTransf+" (CD_ESTAB, CD_ESTGER, NR_PEDIDO) not in (select CD_ESTAB, CD_ESTGER, NR_PEDIDO from PE_CLASPED where ID_CLASSIF in (" + sdsClassifDist + "))";


        #endregion

        #region Retorna Dados dos Gráficos
        internal IN_MENUAPP GetDadosGrafico(double nidGrafico)
        {
            var con = Conexao.Instance("");
            DbDataReader qryGraf = null;

            try
            {
                string sdsTitulo = "";
                int sflTipoGrafico = 0;

                qryGraf = con.execQuery("select * from IN_MENUAPP where ID_MENUAPP = " + nidGrafico);
                if (qryGraf.HasRows)
                {
                    sdsTitulo = qryGraf.GetString(qryGraf.GetOrdinal("DS_MENU"));
                    sflTipoGrafico = Convert.ToInt16(qryGraf.GetValue(qryGraf.GetOrdinal("FL_TIPOGRAFICO")));
                }

                return new IN_MENUAPP() { DS_MENU = sdsTitulo, FL_TIPOGRAFICO = sflTipoGrafico } ;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!qryGraf.IsClosed) qryGraf.Close();
                con.fechaCon();
            }
        }

        internal List<double> RecuperaFaturamentoCarteira(List<string> args)
        {
            Conexao con = Conexao.Instance(null);
            try
            {
                string sdsSql = null;
                string sdsFiltro = "";
                double nvlFatura1 = 0;
                double nvlFatura2 = 0;
                double nvlFatura3 = 0;

                if (args[0] == "Consolidado") sdsFiltro = sdsConsolidado;
                if (args[0] == "Artefatos") sdsFiltro = sdsFiltroArtefatos;
                if (args[0] == "Transformadores") sdsFiltro = sdsFiltroTransf;
                if (args[0] == "Ferragens") sdsFiltro = sdsFiltroFerragensNF;
                if (args[0] == "Onix") sdsFiltro = sdsFiltroOnix;

                var mes = args[2];
                var ano = args[3];
                string mesAno = mes + "/" + ano;
                string ddtIni = "01/" + mesAno;
                DateTime data = new DateTime(int.Parse(ano), int.Parse(mes), 1);
                data = data.LastDayOfMonth();
                string ddtFim = data.Day + "/" + mesAno;

                sdsSql = "select " +
    "(select sum(VL_TNFISC) from FT_NFISC where DT_EMISNF between to_date('" + ddtIni + "','dd/mm/yyyy') and to_date('" + ddtFim + "','dd/mm/yyyy') and DT_CANCEL is null and " + sdsFiltro + " CD_MERCADO = 100 and(CD_NATOP, CD_NATOPIT) in(select CD_NATOP, CD_NATOPIT from CT_ITNAT where BO_CARTEIR = 'T')) as VL_FATUR1, " +
    "(select sum(VL_TNFISC) from FT_NFISC where DT_EMISNF between to_date('" + ddtIni + "','dd/mm/yyyy') and to_date('" + ddtFim + "','dd/mm/yyyy') and DT_CANCEL is null and " + sdsFiltro + " CD_MERCADO = 200 and(CD_NATOP, CD_NATOPIT) in(select CD_NATOP, CD_NATOPIT from CT_ITNAT where BO_CARTEIR = 'T')) as VL_FATUR2, " +
    "(select sum(VL_TNFISC) from FT_NFISC where DT_EMISNF between to_date('" + ddtIni + "','dd/mm/yyyy') and to_date('" + ddtFim + "','dd/mm/yyyy') and DT_CANCEL is null and " + sdsFiltro + " CD_MERCADO = 300 and(CD_NATOP, CD_NATOPIT) in(select CD_NATOP, CD_NATOPIT from CT_ITNAT where BO_CARTEIR = 'T')) as VL_FATUR3 " +
    "from dual";
                dataR = con.execQuery(sdsSql);

                if (dataR.HasRows)
                {
                    while (dataR.Read())
                    {
                        if(!dataR.IsDBNull(dataR.GetOrdinal("VL_FATUR1")))
                            nvlFatura1 += Convert.ToDouble(dataR.GetValue(dataR.GetOrdinal("VL_FATUR1")));
                        if (!dataR.IsDBNull(dataR.GetOrdinal("VL_FATUR2")))
                            nvlFatura2 += Convert.ToDouble(dataR.GetValue(dataR.GetOrdinal("VL_FATUR2")));
                        if (!dataR.IsDBNull(dataR.GetOrdinal("VL_FATUR3")))
                            nvlFatura3 += Convert.ToDouble(dataR.GetValue(dataR.GetOrdinal("VL_FATUR3")));
                    }
                    var nvlTotal = nvlFatura1 + nvlFatura2 + nvlFatura3;
                    retorno = new List<Double> { nvlFatura1, nvlFatura2, nvlFatura3, nvlTotal };
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dataR != null)
                {
                    if (!dataR.IsClosed) dataR.Close();
                }
                con.fechaCon();
            }
            return retorno;
        }
        
        internal List<double> RecuperaOrcamCarteira(List<string> args)
        {
            Conexao con = Conexao.Instance(null);
            try
            {
                string sdsSql = null;
                string sdsFiltro = "";

                double nvlOrcam1 = 0;
                double nvlOrcam2 = 0;
                double nvlOrcam3 = 0;

                if (args[0] == "Consolidado") sdsFiltro = sdsConsolidado;
                if (args[0] == "Artefatos") sdsFiltro = sdsFiltroArtefatos;
                if (args[0] == "Transformadores") sdsFiltro = sdsFiltroTransf;
                if (args[0] == "Ferragens") sdsFiltro = sdsFiltroFerragens;
                if (args[0] == "Onix") sdsFiltro = sdsFiltroOnix;

                var mes = args[2];
                var ano = args[3];
                var mesAno = mes + "/" + ano;

                sdsSql = "select sum(nvl(VL_FATURA,0)) as VL_FATURA,CD_MERCADO " +
" from CM_PARAMCF where FL_TPVENDA = 0 and " + sdsFiltro + " CD_MERCADO in(100,200,300) and DT_REFEREN = to_date('01/" + mesAno + "','dd/mm/yyyy') " +
" group by CD_MERCADO order by CD_MERCADO";
                dataR = con.execQuery(sdsSql);

                if (dataR.HasRows)
                {
                    while (dataR.Read())
                    {
                        double nvlOrcam = 0;
                        int ncdMercado = 0;
                        if (!dataR.IsDBNull(dataR.GetOrdinal("VL_FATURA")))
                            nvlOrcam = Convert.ToInt64(dataR.GetValue(dataR.GetOrdinal("VL_FATURA")));
                        if (!dataR.IsDBNull(dataR.GetOrdinal("CD_MERCADO")))
                            ncdMercado = Convert.ToInt32(dataR.GetValue(dataR.GetOrdinal("CD_MERCADO")));

                        if (ncdMercado == 100) nvlOrcam1 = nvlOrcam;
                        if (ncdMercado == 200) nvlOrcam2 = nvlOrcam;
                        if (ncdMercado == 300) nvlOrcam3 = nvlOrcam;
                    }
                    var nvlTotal = nvlOrcam1 + nvlOrcam2 + nvlOrcam3;
                    retorno = new List<Double> { nvlOrcam1, nvlOrcam2, nvlOrcam3, nvlTotal};
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dataR != null)
                {
                    if (!dataR.IsClosed) dataR.Close();
                }
                con.fechaCon();
            }

            return retorno;
        }

        internal List<double> RecuperaPedidoCarteira(List<string> args)
        {
            Conexao con = Conexao.Instance(null);
            try
            {
                string sdsFiltro = "";
                string sdsFiltroLinha = "";
                string sdsSql = null;

                double nvlPedido1 = 0;
                double nvlPedido2 = 0;
                double nvlPedido3 = 0;

                List<double> lstFaturado = new List<double>();

                if (args[0] == "Consolidado") sdsFiltro = sdsConsolidado;
                if (args[0] == "Artefatos")
                {
                    sdsFiltro = sdsFiltroArtefatos;
                    if (args[1] == "Distribuicao")
                    {
                        sdsFiltroLinha = @" and (CD_ESTAB,CD_GEMAT,CD_CLASSE) in(select CD_ESTAB,CD_GEMAT,CD_CLASSE from MT_MATER where (CD_ESTAB,CD_FAMILIA) in(
    select CD_ESTAB,CD_FAMILIA from MT_FAMIL where BO_ATIVO = 'T' and (CD_ESTAB,CD_FAMILIA) in(select CD_ESTAB, CD_FAMILIA from MT_FAMGP where
        ID_GPFAMIL in(select ID_GPFAMIL from MT_GPFAMIL where DT_INATIVO is null and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where
            DT_INATIVO is null and CD_ESTGER in (13,15,37,38,41,44,46))))))";
                    }
                }
                if (args[0] == "Transformadores")
                {
                    sdsFiltro = sdsFiltroTransf;
                    string sdsAux = "";
                    if (args[1] == "Distribuicao")
                        sdsAux = "CD_ESTGER = 8 and ID_LINGEN in(2)";
                    else if (args[1] == "Industrial")
                        sdsAux = "CD_ESTGER = 8 and ID_LINGEN in(7,22)";
                    else if (args[1] == "Consolidado")
                        sdsAux = "CD_ESTGER = 8 and ID_LINGEN in(2,7,22)";
                    if (sdsAux != "")
                    {
                        sdsFiltroLinha = @" and (CD_ESTAB,CD_GEMAT,CD_CLASSE) in(select CD_ESTAB,CD_GEMAT,CD_CLASSE from MT_MATER where (CD_ESTAB,CD_FAMILIA) in(
    select CD_ESTAB,CD_FAMILIA from MT_FAMIL where BO_ATIVO = 'T' and (CD_ESTAB,CD_FAMILIA) in(select CD_ESTAB, CD_FAMILIA from MT_FAMGP where
        ID_GPFAMIL in(select ID_GPFAMIL from MT_GPFAMIL where DT_INATIVO is null and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where
            DT_INATIVO is null and " + sdsAux + ")))))";
                    }
                }
                if (args[0] == "Ferragens")
                {
                    sdsFiltro = sdsFiltroFerragens;
                    if (args[1] == "Distribuicao")
                    {
                        sdsFiltroLinha = @" and (CD_ESTAB,CD_GEMAT,CD_CLASSE) in(select CD_ESTAB,CD_GEMAT,CD_CLASSE from MT_MATER where (CD_ESTAB,CD_FAMILIA) in(
    select CD_ESTAB,CD_FAMILIA from MT_FAMIL where BO_ATIVO = 'T' and (CD_ESTAB,CD_FAMILIA) in(select CD_ESTAB, CD_FAMILIA from MT_FAMGP where
        ID_GPFAMIL in(select ID_GPFAMIL from MT_GPFAMIL where DT_INATIVO is null and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where
            DT_INATIVO is null and CD_ESTGER = 2 and ID_LINGEN in (14,16,18,19,26))))))";
                    }
                }
                if (args[0] == "Onix")
                {
                    string sdsAux = "";
                    sdsFiltro = sdsFiltroOnix;
                    if(args[1] != "")
                    {                        
                        if (args[1] == "Distribuicao")
                            sdsAux = "CD_ESTGER in(85,86) and ID_LINGEN not in(7)";
                        else if (args[1] == "Industrial")
                            sdsAux = "CD_ESTGER in(85,86) and ID_LINGEN in(7)";
                        else if (args[1] == "Consolidado")
                            sdsAux = "CD_ESTGER in(85,86)";
                    }
                    if (sdsAux != "")
                    {
                        sdsFiltroLinha = @" and (CD_ESTAB,CD_GEMAT,CD_CLASSE) in(select CD_ESTAB,CD_GEMAT,CD_CLASSE from MT_MATER where (CD_ESTAB,CD_FAMILIA) in(
    select CD_ESTAB,CD_FAMILIA from MT_FAMIL where BO_ATIVO = 'T' and (CD_ESTAB,CD_FAMILIA) in(select CD_ESTAB, CD_FAMILIA from MT_FAMGP where
        ID_GPFAMIL in(select ID_GPFAMIL from MT_GPFAMIL where DT_INATIVO is null and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where
            DT_INATIVO is null and " + sdsAux + ")))))";
                    }
                }

                var mes = args[2];
                var ano = args[3];
                string mesAno = mes + "/" + ano;
               
                DateTime data = new DateTime(int.Parse(ano), int.Parse(mes), 1);
                data = data.LastDayOfMonth();
                string ddtFim = data.Day + "/" + mesAno;
                data = data.FirstDayOfMonth();

                if ((int.Parse(mes) == DateTime.Now.Month && int.Parse(ano) == DateTime.Now.Year) || data < DateTime.Now) data = data.AddMonths(-3);
                string ddtIni = "01/"+data.Month+"/"+data.Year;

                sdsSql = "select sum(VL_TOTAL) as VL_PEDIDO, CD_MERCADO from VCM_PEDF where " +
  "DT_FATURA between to_date('" + ddtIni + "','dd/mm/yyyy') and to_date('" + ddtFim + "','dd/mm/yyyy') and " + sdsFiltro +
      "CD_SITDOC not in(2, 4, 6, 7, 8, 14) and " +
           "(CD_NATOP, CD_NATOPIT) in(select CD_NATOP, CD_NATOPIT from CT_ITNAT where BO_CARTEIR = 'T') " + sdsFiltroLinha +
  " group by CD_MERCADO";
                dataR = con.execQuery(sdsSql);

                if (dataR.HasRows)
                {
                    while (dataR.Read())
                    {
                        double nvlPedido = 0;
                        double ncdMercado = 0;
                        if (!dataR.IsDBNull(dataR.GetOrdinal("VL_PEDIDO")))
                            nvlPedido = Convert.ToDouble(dataR.GetValue(dataR.GetOrdinal("VL_PEDIDO")));
                        if (!dataR.IsDBNull(dataR.GetOrdinal("CD_MERCADO")))
                            ncdMercado = Convert.ToInt32(dataR.GetValue(dataR.GetOrdinal("CD_MERCADO")));

                        if (ncdMercado == 100) nvlPedido1 = nvlPedido;
                        if (ncdMercado == 200) nvlPedido2 = nvlPedido;
                        if (ncdMercado == 300) nvlPedido3 = nvlPedido;
                    }

                    lstFaturado = RecuperaFaturamentoCarteira(args);

                    if (sdsFiltroLinha == "")
                    {
                        nvlPedido1 += lstFaturado[0];
                        nvlPedido2 += lstFaturado[1];
                        nvlPedido3 += lstFaturado[2];
                    }

                    var nvlTotal = nvlPedido1 + nvlPedido2 + nvlPedido3;
                    retorno = new List<Double> { nvlPedido1, nvlPedido2, nvlPedido3, nvlTotal };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (dataR != null) {
                    if (!dataR.IsClosed) dataR.Close();
                }
                con.fechaCon();
            }
            return retorno;
        }

        internal List<double> RecuperaFaturamentoMercado(List<string> args)
        {
            try
            {
                List<double> nvlMercado = new List<double>();
                List<double> lstFaturado = new List<double>();
                if (args[2] == "")
                {
                    double nvlM100 = 0;
                    double nvlM200 = 0;
                    double nvlM300 = 0;

                    var mes = 1;
                    while (mes <= 12)
                    {
                        args[2] = "" + mes;
                        lstFaturado = RecuperaFaturamentoCarteira(args);

                        nvlM100 += lstFaturado[0];
                        nvlM200 += lstFaturado[1];
                        nvlM300 += lstFaturado[2];

                        mes++;
                    }

                    nvlMercado.Add(nvlM100);
                    nvlMercado.Add(nvlM200);
                    nvlMercado.Add(nvlM300);
                }
                else {
                    lstFaturado = RecuperaFaturamentoCarteira(args);
                    nvlMercado.Add(lstFaturado[0]);
                    nvlMercado.Add(lstFaturado[1]);
                    nvlMercado.Add(lstFaturado[2]);
                }
                
                return nvlMercado;
            }
            catch { throw; }
        }

        internal List<double> RecuperaOrcamMesAMes(List<string> args)
        {
            try
            {
                var mes = 1;
                var ano = args[3];
                List<double> nvlOrcam = new List<double>();

                while (mes <= 12)
                {
                    args[2] = ""+mes;
                    nvlOrcam.Add(RecuperaOrcamCarteira(args)[3]);
                    mes++;
                }

                return nvlOrcam;
            }
            catch { throw; }
        }

        internal List<double> RecuperaFaturamentoMesAMes(List<string> args)
        {
            try
            {
                var mes = 1;
                var ano = args[3];
                List<double> nvlFatur = new List<double>();

                while (mes <= 12)
                {
                    args[2] = "" + mes;
                    nvlFatur.Add(RecuperaFaturamentoCarteira(args)[3]);
                    mes++;
                }

                return nvlFatur;
            }
            catch { throw; }
        }

        internal List<double> RecuperaPedidosEmitidosAno(List<string> args)
        {
            Conexao con = Conexao.Instance(null);
            try
            {
                List<double> retorno = new List<double>();
                string sdsFiltro = null;

                if (args[0] == "Distribuicao") sdsFiltro = sdsFiltroDistribuicao;
                if (args[0] == "Industrial") sdsFiltro = sdsFiltroIndustrial;                
                if (args[0] == "Onix") sdsFiltro = sdsFiltroOnix.Replace(")) and", ")) ");
                if (args[0] == "Consolidado") sdsFiltro = sdsFiltroOnix.Replace(")) and",")) or ") + sdsFiltroDistribuicao + " or " + sdsFiltroIndustrial;

                var mes = 1;//args[2]
                var ano = args[3];
                string mesAno = mes + "/" + ano;
                string ddtIni = "01/"+mesAno;

                string ddtFim = "01/01/" + Convert.ToString(Convert.ToInt16(args[3])+1);

                string sdsSql = @"select to_char(DT_EMISPED, 'MM') as MES, count(*) as QTDE from PE_PEDID where DT_EMISPED >= to_date('" + ddtIni + @"','dd/mm/yyyy') and DT_EMISPED < to_date('" + ddtFim + @"','dd/mm/yyyy')
                     and CD_SITDOC not in(6,7) and (CD_NATOP, CD_NATOPIT) in(select CD_NATOP, CD_NATOPIT from CT_ITNAT where BO_CARTEIR = 'T') and ( " + sdsFiltro + @" )
                     group by to_char(DT_EMISPED, 'MM') order by to_char(DT_EMISPED, 'MM')";
                dataR = con.execQuery(sdsSql);

                if (dataR.HasRows)
                {
                    while (dataR.Read())
                    {                        
                        if (retorno.Count + 1 != Convert.ToInt16(dataR.GetString(dataR.GetOrdinal("MES"))))
                        {
                            for (int i = retorno.Count + 1; i < Convert.ToInt16(dataR.GetString(dataR.GetOrdinal("MES"))); i++)
                            {
                                retorno.Add(0);
                            }
                        }
                        retorno.Add(Convert.ToInt32(dataR.GetValue(dataR.GetOrdinal("QTDE"))));
                    }

                    if (retorno.Count < 12)
                    {
                        for (int i = retorno.Count + 1; i <= 12; i++)
                        {
                            retorno.Add(0);
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
                if (dataR != null)
                    if (!dataR.IsClosed) dataR.Close();
                con.fechaCon();
            }
        }

        internal List<double> RecuperaPedidosEmitidosMes(List<string> args)
        {
            var con = Conexao.Instance(null);
            try
            {
                List<double> retorno = new List<double>();

                string sdsFiltro = null;

                if (args[0] == "Distribuicao") sdsFiltro = sdsFiltroDistribuicao;
                if (args[0] == "Industrial") sdsFiltro = sdsFiltroIndustrial;
                if (args[0] == "Onix") sdsFiltro = sdsFiltroOnix.Replace(")) and ", " )) ");
                if (args[0] == "Consolidado") sdsFiltro = sdsFiltroOnix.Replace(")) and", ")) or ") + sdsFiltroDistribuicao + " or " + sdsFiltroIndustrial;

                var mes = args[2];
                var ano = args[3];
                string mesAno = mes + "/" + ano;
                string ddtIni = "01/" + mesAno;

                DateTime data = new DateTime(int.Parse(ano), int.Parse(mes), 1);
                data = data.LastDayOfMonth().AddDays(1);
                string ddtFim = data.Day + "/" + data.Month + "/" + data.Year;

                string sdsSql = @"select to_char(DT_EMISPED, 'DD') as DIA, count(*) as QTDE from PE_PEDID where DT_EMISPED >= to_date('" + ddtIni + @"','dd/mm/yyyy') and DT_EMISPED < to_date('" + ddtFim + @"','dd/mm/yyyy') 
                     and CD_SITDOC not in(6,7) and (CD_NATOP, CD_NATOPIT) in(select CD_NATOP, CD_NATOPIT from CT_ITNAT where BO_CARTEIR = 'T') and (" + sdsFiltro + @")
                     group by to_char(DT_EMISPED, 'DD') order by to_char(DT_EMISPED, 'DD')";
                dataR = con.execQuery(sdsSql);

                if (dataR.HasRows)
                {
                    while (dataR.Read())
                    {
                        if (retorno.Count + 1 != Convert.ToInt16(dataR.GetString(dataR.GetOrdinal("DIA"))))
                        {
                            for (int i = retorno.Count + 1; i < Convert.ToInt16(dataR.GetString(dataR.GetOrdinal("DIA"))); i++){
                                retorno.Add(0);
                            }
                        }

                        retorno.Add(Convert.ToInt32(dataR.GetValue(dataR.GetOrdinal("QTDE"))));                        
                    }
                }

                if (retorno.Count < 31)
                {
                    for (int i = retorno.Count + 1; i <= 31; i++)
                    {
                        retorno.Add(0);
                    }
                }

                return retorno;
            }
            catch
            {
                throw;
            }finally
            {
                if (dataR != null)
                    if (!dataR.IsClosed) dataR.Close();
                con.fechaCon();
            }
        }

        internal List<double> ConverteDiaSemana(List<double> dias)
        {
            List<double> retorno = new List<double>();
            int diaSemana = 1;
            double vlSemana = 0;
            foreach(double dia in dias)
            {
                vlSemana = vlSemana + dia;
                if (diaSemana == 7)
                {
                    retorno.Add(vlSemana);
                    vlSemana = 0;
                    diaSemana = 0;
                }
                diaSemana++;
            }
            retorno.Add(vlSemana); //A ultima semana sempre será atribuida fora do for.

            return retorno;
        }

        internal List<double> RecuperaPedidosCarteira(List<String> args)
        {
            var con = Conexao.Instance(null);
            try
            {
                var retorno = new List<double>();

                string sdsFiltro = null;

                if (args[0] == "Distribuicao") sdsFiltro = sdsFiltroDistribuicao;
                if (args[0] == "Industrial") sdsFiltro = sdsFiltroIndustrial;
                if (args[0] == "Onix") sdsFiltro = sdsFiltroOnix.Replace(")) and ", ")) "); ;
                if (args[0] == "Consolidado") sdsFiltro = sdsFiltroOnix.Replace(")) and", ")) or ") + sdsFiltroDistribuicao + " or " + sdsFiltroIndustrial;

                var mes = args[2];
                var ano = args[3];
                string mesAno = mes + "/" + ano;
                string ddtIni = "01/" + mesAno;

                DateTime data = new DateTime(int.Parse(ano), int.Parse(mes), 1);
                data = data.LastDayOfMonth().AddDays(1) ;
                string ddtFim = data.Day + "/" + data.Month + "/" + data.Year;

                string sdsSql = @"select to_char(DT_EMISPED, 'DD') as DIA,
(select count(distinct NR_PEDIDO) from PE_PEDID a where to_char(a.DT_EMISPED,'DD') = to_char(PE_PEDID.DT_EMISPED,'DD') and
CD_SITDOC not in(6,7) and (CD_NATOP, CD_NATOPIT) in(select CD_NATOP, CD_NATOPIT from CT_ITNAT where BO_CARTEIR = 'T') and (" + sdsFiltro + @" ) and
a.DT_EMISPED >= to_date('" + ddtIni + @"','dd/mm/yyyy') and DT_EMISPED < to_date('" + ddtFim + @"','dd/mm/yyyy') and
  (a.NR_PEDIDO,a.CD_ESTGER,a.CD_ESTAB) in(select NR_PEDIDO,CD_ESTGER,CD_ESTAB from PE_RITDE where
        DT_FATURA >= to_date('" + ddtIni + @"','dd/mm/yyyy') and DT_FATURA < to_date('" + ddtFim + @"','dd/mm/yyyy'))
) as CARTEIRA
from PE_PEDID where DT_EMISPED >= to_date('" + ddtIni + @"','dd/mm/yyyy') and DT_EMISPED < to_date('"+ ddtFim+ @"','dd/mm/yyyy')
and CD_SITDOC not in(6,7) and (CD_NATOP, CD_NATOPIT) in(select CD_NATOP, CD_NATOPIT from CT_ITNAT where BO_CARTEIR = 'T') and (" + sdsFiltro + @")
group by to_char(DT_EMISPED, 'DD') order by to_char(DT_EMISPED, 'DD')";

                dataR = con.execQuery(sdsSql);

                if (dataR.HasRows)
                {
                    while (dataR.Read())
                    {
                        if (retorno.Count + 1 != Convert.ToInt16(dataR.GetString(dataR.GetOrdinal("DIA"))))
                        {
                            for (int i = retorno.Count + 1; i < Convert.ToInt16(dataR.GetString(dataR.GetOrdinal("DIA"))); i++)
                            {
                                retorno.Add(0);
                            }
                        }

                        retorno.Add(Convert.ToInt32(dataR.GetValue(dataR.GetOrdinal("CARTEIRA"))));
                    }
                }

                if (retorno.Count < 31)
                {
                    for (int i = retorno.Count + 1; i <= 31; i++)
                    {
                        retorno.Add(0);
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
                if (dataR != null)
                    if (!dataR.IsClosed) dataR.Close();
                con.fechaCon();
            }
        }

        internal List<double> RecuperaOrcamEstabLinha(List<string> args)
        {
            var con = Conexao.Instance(null);
            try
            {
                string sdsFiltro = "";
                string sdsEstabs = "";

                if (args[0] == "Ferragens")
                {
                    sdsEstabs = "CD_ESTAB in (2,35,42)";
                    sdsFiltro = "and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where CD_ESTGER = 2 and ID_LINGEN in (14,16,18,19,26))";
                };
                if (args[0] == "Transformadores")
                {
                    sdsEstabs = "CD_ESTAB in (8)";
                    if (args[1] == "Distribuicao")
                        sdsFiltro = "and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where CD_ESTGER = 8 and ID_LINGEN in(2))";
                    else if (args[1] == "Industrial")
                        sdsFiltro = "and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where CD_ESTGER = 8 and ID_LINGEN in(7,22))";
                    else if (args[1] == "Consolidado")
                        sdsFiltro = "and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where CD_ESTGER = 8 and ID_LINGEN in(2,7,22))";
                }
                if(args[0] == "Artefatos")
                {
                    sdsEstabs = "CD_ESTAB in (13,15,37,38,41,44,46)";
                    sdsFiltro = "and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where CD_ESTGER in (13,15,37,38,41,44,46))";
                }
                if (args[0] == "Onix")
                {
                    sdsEstabs = "CD_ESTAB in(85,86)";
                    if (args[1] == "Distribuicao")
                        sdsFiltro = "and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where CD_ESTGER in(85,86) and ID_LINGEN not in(7))";
                    else if (args[1] == "Industrial")
                        sdsFiltro = "and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where CD_ESTGER in(85,86) and ID_LINGEN in(7))";
                    else if (args[1] == "Consolidado")
                        sdsFiltro = "and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where CD_ESTGER in(85,86))";
                }

                string mesAno = args[2] + "/" + args[3];

                List<double> retorno = new List<double>();

                var sdsSql = @"select sum(VL_COMPRO) as COMPRO from SG_COMPFAMIL where ID_COMPRO in(
select ID_COMPRO from SG_COMPRO where DT_REFEREN = to_date('01/" + mesAno + @"','dd/mm/yyyy') and ID_SITCOMPRO in(
select ID_SITCOMPRO from SG_SITCOMPRO where ID_REVISAO in(select ID_REVISAO from SG_REVISAO where ID_ORCAM in(select ID_ORCAM from SG_ORCAM where CD_EMPRESA in(1, 85) and NR_ANO = " + args[3] + @"))
and (" + sdsEstabs + ") and CD_MERCADO = 200)) " + sdsFiltro;

                dataR = con.execQuery(sdsSql);

                if (dataR.HasRows)
                {
                    dataR.Read();
                    retorno.Add(Convert.ToInt64(dataR.GetValue(dataR.GetOrdinal("COMPRO"))));                    
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

        internal List<double> RecuperaFaturadoEstabLinha(List<string> args)
        {
            var con = Conexao.Instance(null);
            try
            {
                List<double> retorno = new List<double>();

                string sdsFiltro = "";
                string sdsEstabs = "";

                if (args[0] == "Ferragens")
                {
                    sdsEstabs = sdsFiltroFerragens;
                    sdsFiltro = "CD_ESTGER = 2 and ID_LINGEN in (14,16,18,19,26)";
                };
                if (args[0] == "Transformadores")
                {
                    sdsEstabs = sdsFiltroTransf;
                    if (args[1] == "Distribuicao")
                        sdsFiltro = "CD_ESTGER = 8 and ID_LINGEN in(2)";
                    else if (args[1] == "Industrial")
                        sdsFiltro = "CD_ESTGER = 8 and ID_LINGEN in(7,22)";
                    else if (args[1] == "Consolidado")
                        sdsFiltro = "CD_ESTGER = 8 and ID_LINGEN in(2,7,22)";
                }
                if (args[0] == "Artefatos")
                {
                    sdsEstabs = sdsFiltroArtefatos;
                    sdsFiltro = "CD_ESTGER in (13,15,37,38,41,44,46)";
                }
                if (args[0] == "Onix")
                {
                    sdsEstabs = sdsFiltroOnix;
                    if (args[1] == "Distribuicao")
                        sdsFiltro = "CD_ESTGER in(85,86) and ID_LINGEN not in(7)";
                    else if (args[1] == "Industrial")
                        sdsFiltro = "CD_ESTGER in(85,86) and ID_LINGEN in(7)";
                    else if (args[1] == "Consolidado")
                        sdsFiltro = "CD_ESTGER in(85,86)";
                }

                sdsEstabs = sdsEstabs.Replace("CD_", "a.CD_");

                var mes = args[2];
                var ano = args[3];
                string mesAno = mes + "/" + ano;
                string ddtIni = "01/" + mesAno;

                DateTime data = new DateTime(int.Parse(ano), int.Parse(mes), 1);
                data = data.LastDayOfMonth();
                string ddtFim = data.Day + "/" + mesAno;

                var sdsSql = @"select sum(VL_TNFISC) as VALOR from FT_NFISC where (CD_ESTAB,CD_ESTGER,NR_SERIENF,NR_NFISCAL,DT_EMISNF) in(
select distinct a.CD_ESTAB,a.CD_ESTGER,a.NR_SERIENF,a.NR_NFISCAL,a.DT_EMISNF from  FT_NFISC a, FT_ITNFI b where
a.CD_ESTAB = b.CD_ESTAB and
a.CD_ESTGER = b.CD_ESTGER and
a.NR_SERIENF = b.NR_SERIENF and
a.NR_NFISCAL = b.NR_NFISCAL and
a.DT_EMISNF = b.DT_EMISNF and
a.DT_CANCEL is null and
(a.CD_NATOP, a.CD_NATOPIT) in(select CD_NATOP, CD_NATOPIT from CT_ITNAT where BO_CARTEIR = 'T') and
a.DT_EMISNF between to_date('" + ddtIni + @"','dd/mm/yyyy') and to_date('" + ddtFim + @"','dd/mm/yyyy') and
a.CD_MERCADO = 200 and " + sdsEstabs + @" 
(b.CD_ESTAB, b.CD_GEMAT, b.CD_CLASSE) in(select CD_ESTAB, CD_GEMAT, CD_CLASSE from MT_MATER where (CD_ESTAB, CD_FAMILIA) in(
     select CD_ESTAB, CD_FAMILIA from MT_FAMIL where BO_ATIVO = 'T' and(CD_ESTAB, CD_FAMILIA) in(select CD_ESTAB, CD_FAMILIA from MT_FAMGP where
          ID_GPFAMIL in(select ID_GPFAMIL from MT_GPFAMIL where DT_INATIVO is null and ID_LINPROD in(select ID_LINPROD from MT_LINPROD where
            DT_INATIVO is null and " + sdsFiltro + @"))))))";

                dataR = con.execQuery(sdsSql);
                if(dataR.HasRows)
                {
                    retorno.Add(Convert.ToInt64(dataR.GetValue(dataR.GetOrdinal("VALOR"))));
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
        #endregion

        #region Retorna Classe Grafico do Novo Formato de Gráficos
        [HttpPost]
        [Route("GetCarteiraEstabLinha")]
        [AllowAnonymous]
        public Grafico GetCarteiraEstabLinha(List<string> args, double nidMenuAPP)
        {
            try
            {
                var inMenuAPP = GetDadosGrafico(nidMenuAPP);
                Grafico graf = new Grafico()
                {
                    ID_MENUAPP = nidMenuAPP,
                    sdsTitulo = inMenuAPP.DS_MENU,
                    sdsTipoGrafico = inMenuAPP.FL_TIPOGRAFICO
                };

                List<double> retornoWS = new List<double>();

                List<double> lstOrcamento = new List<double>();
                List<double> lstFaturado = new List<double>();
                List<double> lstCarteira = new List<double>();

                args[1] = "Industrial";
                args[0] = "Transformadores";
                retornoWS = RecuperaOrcamEstabLinha(args);
                var nvlOrcamTransfInd = retornoWS[0];
                retornoWS = RecuperaFaturadoEstabLinha(args);
                var nvlFaturTransfInd = retornoWS[0];
                retornoWS = RecuperaPedidoCarteira(args);
                var nvlCarteiraTransfInd = nvlFaturTransfInd + retornoWS[1];

                lstOrcamento.Add(nvlOrcamTransfInd);
                lstCarteira.Add(nvlCarteiraTransfInd);
                lstFaturado.Add(nvlFaturTransfInd);                

                args[1] = "Distribuicao";
                retornoWS = RecuperaOrcamEstabLinha(args);
                var nvlOrcamTransf = retornoWS[0];
                retornoWS = RecuperaFaturadoEstabLinha(args);
                var nvlFaturTransf = retornoWS[0];
                retornoWS = RecuperaPedidoCarteira(args);
                var nvlCarteiraTransf = nvlFaturTransf + retornoWS[1];

                args[0] = "Artefatos";
                retornoWS = RecuperaOrcamEstabLinha(args);
                var nvlOrcamArtef = retornoWS[0];
                retornoWS = RecuperaFaturadoEstabLinha(args);
                var nvlFaturArtef = retornoWS[0];
                retornoWS = RecuperaPedidoCarteira(args);
                var nvlCarteiraArtef = nvlFaturArtef + retornoWS[1];

                args[0] = "Ferragens";
                retornoWS = RecuperaOrcamEstabLinha(args);
                var nvlOrcamFer = retornoWS[0];
                retornoWS = RecuperaFaturadoEstabLinha(args);
                var nvlFaturFer = retornoWS[0];
                retornoWS = RecuperaPedidoCarteira(args);
                var nvlCarteiraFer = nvlFaturFer + retornoWS[1];

                lstOrcamento.Add(nvlOrcamArtef + nvlOrcamFer + nvlOrcamTransf);
                lstCarteira.Add(nvlCarteiraArtef + nvlCarteiraFer + nvlCarteiraTransf);
                lstFaturado.Add(nvlFaturArtef + nvlFaturFer + nvlFaturTransf);

                args[0] = "Onix";
                retornoWS = RecuperaOrcamEstabLinha(args);
                var nvlOrcamOnix = retornoWS[0];
                retornoWS = RecuperaFaturadoEstabLinha(args);
                var nvlFaturOnix = retornoWS[0];
                retornoWS = RecuperaPedidoCarteira(args);
                var nvlCarteiraOnix = nvlFaturOnix + retornoWS[1];

                lstOrcamento.Add(nvlOrcamOnix);
                lstCarteira.Add(nvlCarteiraOnix);
                lstFaturado.Add(nvlFaturOnix);

                var nvlOramConsol = nvlOrcamArtef + nvlOrcamFer + nvlOrcamTransf + nvlOrcamTransfInd + nvlOrcamOnix;
                var nvlFaturConsol = nvlFaturArtef + nvlFaturFer + nvlFaturTransf + nvlFaturTransfInd + nvlFaturOnix;
                var nvlCartConsol = nvlCarteiraArtef + nvlCarteiraFer + nvlCarteiraTransf + nvlCarteiraTransfInd + nvlCarteiraOnix;

                lstOrcamento.Add(nvlOramConsol);
                lstCarteira.Add(nvlCartConsol);
                lstFaturado.Add(nvlFaturConsol);

                graf.Dados = new List<DadosGrafico>();
                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = lstOrcamento },
                    Label = "Orçamento",
                    cor = new CorGraf(54, 56, 217, 200)
                });
                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = lstCarteira },
                    Label = "Carteira",
                    cor = new CorGraf(0, 160, 50, 200)
                });
                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = lstFaturado },
                    Label = "Faturado",
                    cor = new CorGraf(178, 47, 32, 200)
                });
                
                graf.Legendas = new List<string> { "Industrial", "Distribuição", "Onix", "Consolidado" };
                return graf;
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [Route("GetPedidosEmitidosAno")]
        [AllowAnonymous]
        public Grafico GetPedidosEmitidosAno(List<string> args, double nidMenuAPP)
        {
            try
            {
                var inMenuAPP = GetDadosGrafico(nidMenuAPP);
                Grafico graf = new Grafico()
                {
                    ID_MENUAPP = nidMenuAPP,
                    sdsTitulo = inMenuAPP.DS_MENU,
                    sdsTipoGrafico = inMenuAPP.FL_TIPOGRAFICO
                };

                var argsAux = new List<string> { args[0], "", "", DateTime.Now.Year.ToString() };
                if(args[3] == argsAux[3])                
                    args[3] = Convert.ToString(DateTime.Now.Year - 1);//Muda o ano de comparação caso esteja igual                

                var anoCompara = RecuperaPedidosEmitidosAno(args);
                var anoAtual = RecuperaPedidosEmitidosAno(argsAux);

                graf.Dados = new List<DadosGrafico>();
                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = anoAtual },
                    Label = "Ano Atual",
                    cor = new CorGraf(56,55,227, 200)
                });
                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = anoCompara },
                    Label = "Ano de [ " + args[3] +" ]",
                    cor = new CorGraf(0, 225, 50, 200)
                });

                graf.Legendas = new List<string> { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez" };

                return graf;
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [Route("GetPedidosCarteira")]
        public Grafico GetPedidosCarteira(List<string> args, double nidMenuAPP)
        {
            try
            {
                var inMenuAPP = GetDadosGrafico(nidMenuAPP);
                Grafico graf = new Grafico()
                {
                    ID_MENUAPP = nidMenuAPP,
                    sdsTitulo = inMenuAPP.DS_MENU,
                    sdsTipoGrafico = inMenuAPP.FL_TIPOGRAFICO
                };

                var qtdPedido = RecuperaPedidosEmitidosMes(args);
                var qtdCarteira = RecuperaPedidosCarteira(args);
                graf.Dados = new List<DadosGrafico>();

                qtdPedido = ConverteDiaSemana(qtdPedido);
                qtdCarteira = ConverteDiaSemana(qtdCarteira);

                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = qtdPedido },
                    Label = "Pedidos Emitidos",
                    cor = new CorGraf(54, 56, 217, 200)
                });
                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = qtdCarteira },
                    Label = "Carteira do Mês",
                    cor = new CorGraf(0, 160, 50, 200)
                });

                List<String> lgd = new List<string>();
                for (int i = 1; i <= qtdCarteira.Count; i++)
                {
                    lgd.Add("Sem " + i.ToString());
                }
                graf.Legendas = lgd;
                return graf;
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [Route("GetPedidosEmitidosMes")]
        public Grafico GetPedidosEmitidosMes(List<string> args, double nidMenuAPP)
        {
            try
            {
                var inMenuAPP = GetDadosGrafico(nidMenuAPP);
                Grafico graf = new Grafico()
                {
                    ID_MENUAPP = nidMenuAPP,
                    sdsTitulo = inMenuAPP.DS_MENU,
                    sdsTipoGrafico = inMenuAPP.FL_TIPOGRAFICO
                };

                List<String> argsAux = new List<string> { args[0], "", DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString() };
                var mesAtual = RecuperaPedidosEmitidosMes(argsAux);

                if(args[2] == argsAux[2] && args[3] == argsAux[3])
                {
                    args[2] = Convert.ToString(Convert.ToInt16(args[2]) - 1);
                }
                var mesCompara = RecuperaPedidosEmitidosMes(args);
                graf.Dados = new List<DadosGrafico>();

                mesAtual = ConverteDiaSemana(mesAtual);
                mesCompara = ConverteDiaSemana(mesCompara);

                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = mesAtual },
                    Label = "Mês Atual",
                    cor = new CorGraf(178, 47, 32, 200)
                });
                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = mesCompara },
                    Label = "Mês ["+args[2]+"/"+args[3]+"]",
                    cor = new CorGraf(0, 160, 50, 200)
                });

                List<String> lgd = new List<string>();
                for(int i = 1; i <= mesAtual.Count; i++)
                {
                    lgd.Add("Sem "+i.ToString());
                }
                graf.Legendas = lgd;
                return graf;
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [Route("GetGrafCarteira")]
        public Grafico GetGrafCarteira(List<string> args, double nidMenuAPP)
        {
            try
            {
                var inMenuAPP = GetDadosGrafico(nidMenuAPP);

                Grafico graf = new Grafico()
                {
                    ID_MENUAPP = nidMenuAPP,
                    sdsTitulo = inMenuAPP.DS_MENU,
                    sdsTipoGrafico = inMenuAPP.FL_TIPOGRAFICO                    
                };

                List<Double> lstFatura = null;
                List<Double> lstPedido = null;
                List<Double> lstOrcam = null;

                lstPedido = RecuperaPedidoCarteira(args);
                lstFatura = RecuperaFaturamentoCarteira(args);
                lstOrcam = RecuperaOrcamCarteira(args);

                List<List<double>> retorno = new List<List<double>>();

                graf.Dados = new List<DadosGrafico>();

                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = lstOrcam },
                    Label = "Orçamento",
                    cor = new CorGraf(0,112,192,200)// Color.FromArgb(200, 0, 112, 192)
                });
                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = lstPedido },
                    Label = "Pedido",
                    cor = new CorGraf(0,176,80,200)// Color.FromArgb(200, 0, 176, 80)
                });
                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = lstFatura},
                    Label = "Faturado",
                    cor = new CorGraf(142,94,162,200)// Color.FromArgb(200, 142, 94, 162)
                });                
                graf.Legendas = new List<string> { "Concessionária", "Privado", "Exportação", "Total" };
                return graf;
            }
            catch
            {
                throw;
            }
        }
        
        [HttpPost]
        [Route("GetGrafCarteiraGlobal")]
        public Grafico GetGrafCarteiraGlobal(List<string> args, double nidMenuAPP)
        {
            try
            {
                var inMenuAPP = GetDadosGrafico(nidMenuAPP);

                Grafico graf = new Grafico()
                {
                    ID_MENUAPP = nidMenuAPP,
                    sdsTitulo = inMenuAPP.DS_MENU,
                    sdsTipoGrafico = inMenuAPP.FL_TIPOGRAFICO
                };

                List<Double> lstFatura = null;
                List<Double> lstPedido = null;
                List<Double> lstOrcam = null;

                List<string> aux = new List<string>() { "Ferragens", "", args[2], args[3] };
                var lstFerPedido = RecuperaPedidoCarteira(aux);
                var lstFerFatura = RecuperaFaturamentoCarteira(aux);
                var lstFerOrcam = RecuperaOrcamCarteira(aux);

                aux[0] = "Artefatos";
                var lstArtPedido = RecuperaPedidoCarteira(aux);
                var lstArtFatura = RecuperaFaturamentoCarteira(aux);
                var lstArtOrcam = RecuperaOrcamCarteira(aux);

                aux[0] = "Transformadores";
                var lstTraPedido = RecuperaPedidoCarteira(aux);
                var lstTraFatura = RecuperaFaturamentoCarteira(aux);
                var lstTraOrcam = RecuperaOrcamCarteira(aux);

                aux[0] = "Onix";
                var lstOniPedido = RecuperaPedidoCarteira(aux);
                var lstOniFatura = RecuperaFaturamentoCarteira(aux);
                var lstOniOrcam = RecuperaOrcamCarteira(aux);

                var nvlTotRomaOrcam = lstFerOrcam[3] + lstArtOrcam[3] + lstTraOrcam[3];
                var nvlTotGloablOrcam = lstFerOrcam[3] + lstArtOrcam[3] + lstTraOrcam[3] + lstOniOrcam[3];
                lstOrcam = new List<double>() { lstFerOrcam[3], lstArtOrcam[3], lstTraOrcam[3], nvlTotRomaOrcam, lstOniOrcam[3], nvlTotGloablOrcam };
                var nvlTotRomaPedido = lstFerPedido[3] + lstArtPedido[3] + lstTraPedido[3];
                var nvlTotGloablPedido = lstFerPedido[3] + lstArtPedido[3] + lstTraPedido[3] + lstOniPedido[3];
                lstPedido = new List<double>() { lstFerPedido[3], lstArtPedido[3], lstTraPedido[3], nvlTotRomaPedido, lstOniPedido[3], nvlTotGloablPedido };
                var nvlTotRomaFatura = lstFerFatura[3] + lstArtFatura[3] + lstTraFatura[3];
                var nvlTotGloablFatura = lstFerFatura[3] + lstArtFatura[3] + lstTraFatura[3] + lstOniFatura[3];
                lstFatura = new List<double>() { lstFerFatura[3], lstArtFatura[3], lstTraFatura[3], nvlTotRomaFatura, lstOniFatura[3], nvlTotGloablFatura };

                graf.Dados = new List<DadosGrafico>();

                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = lstOrcam },
                    Label = "Orçamento",
                    cor = new CorGraf(0,112,192,200)// Color.FromArgb(200, 0, 112, 192)
                });
                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value= lstPedido },
                    Label = "Pedido",
                    cor = new CorGraf(0,176,80,200)// Color.FromArgb(200, 0, 176, 80)
                });
                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = lstFatura },
                    Label = "Faturado",
                    cor = new CorGraf(142,94,162,200)// Color.FromArgb(200, 142, 94, 162)
                });
                graf.Legendas = new List<string>() { "Ferrragens", "Artefatos", "Transformador", "Roma", "Onix", "Global" };
                return graf;
            }
            catch { throw; }
        }

        [HttpPost]
        [Route("GetFaturadoAnual")]
        public Grafico GetFaturadoAnual(List<string> args, double nidMenuAPP)
        {
            try
            {
                var inMenuAPP = GetDadosGrafico(nidMenuAPP);

                Grafico graf = new Grafico()
                {
                    ID_MENUAPP = nidMenuAPP,
                    sdsTitulo = inMenuAPP.DS_MENU,
                    sdsTipoGrafico = inMenuAPP.FL_TIPOGRAFICO
                };

                List<Double> lstFatura = null;
                List<Double> lstOrcam = null;

                lstFatura = RecuperaFaturamentoMesAMes(args);
                lstOrcam = RecuperaOrcamMesAMes(args);

                graf.Dados = new List<DadosGrafico>();

                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = lstOrcam },
                    Label = "Orçamento",
                    cor = new CorGraf(0,112,192,200)// Color.FromArgb(200, 0, 112, 192)
                });
                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = lstFatura },
                    Label = "Faturado",
                    cor = new CorGraf(142, 94, 162, 200)// Color.FromArgb(200, 142, 94, 162)
                });
                graf.Legendas = new List<string>() { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez" };
                return graf;
            }
            catch { throw; }
        }

        [HttpPost]
        [Route("GetFaturadoMercado")]
        public Grafico GetFaturadoMercado(List<string> args, double nidMenuAPP)
        {
            try
            {
                var inMenuAPP = GetDadosGrafico(nidMenuAPP);

                Grafico graf = new Grafico()
                {
                    ID_MENUAPP = nidMenuAPP,
                    sdsTitulo = inMenuAPP.DS_MENU,
                    sdsTipoGrafico = inMenuAPP.FL_TIPOGRAFICO
                };

                List<Double> lstMercado = new List<Double>();

                lstMercado = RecuperaFaturamentoMercado(args);

                graf.Dados = new List<DadosGrafico>();

                List<CorGraf> cores = new List<CorGraf>()
                {
                    new CorGraf(0,112,192,200),// Color.FromArgb(200, 0, 112, 192),                    
                    new CorGraf(0,176,80,200),//Color.FromArgb(200, 0, 176, 80),
                    new CorGraf(142,94,162,200)//Color.FromArgb(200, 142, 94, 162)
                };

                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = lstMercado, coresPizza = cores },
                    Label = "Faturado"
                });
                graf.Legendas = new List<string>() { "Concessionária","Privado","Exportação" };
                return graf;
            }
            catch { throw; }
        }

        [HttpPost]
        [Route("GetAtingimentoMeta")]
        public Grafico GetAtingimentoMeta(List<string> args, double nidMenuAPP)
        {
            try
            {
                var inMenuAPP = GetDadosGrafico(nidMenuAPP);

                Grafico graf = new Grafico()
                {
                    ID_MENUAPP = nidMenuAPP,
                    sdsTitulo = inMenuAPP.DS_MENU,
                    sdsTipoGrafico = inMenuAPP.FL_TIPOGRAFICO
                };

                List<Double> lstFatura = new List<Double>();
                List<Double> lstOrcam = new List<Double>();

                lstFatura = RecuperaFaturamentoCarteira(args);
                lstOrcam = RecuperaOrcamCarteira(args);

                var nvlMeta = lstFatura[3] / lstOrcam[3] * 100;

                CorGraf corFaturado = null;
                if (nvlMeta >= 0) corFaturado = new CorGraf(200, 94,195, 200);
                if (nvlMeta > 50) corFaturado = new CorGraf(170,170,0,200);
                if (nvlMeta > 90) corFaturado = new CorGraf(0, 112, 192, 200);

                List<CorGraf> cores = new List<CorGraf>()
                {
                    new CorGraf(0, 176, 80,200),
                    corFaturado
                };

                graf.Dados = new List<DadosGrafico>();

                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = new List<double>() { lstOrcam[3], lstFatura[3] }, coresPizza = cores },
                    Label = "Atingimento de Meta"
                });
                graf.Legendas = new List<string>() { "Orçamento", "Realizado"};

                return graf;
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [Route("GetCotacaoMoeda")]
        public Grafico GetCotacaoMoeda (List<string> args, double nidMenuAPP)
        {
            try
            {
                var inMenuAPP = GetDadosGrafico(nidMenuAPP);

                Grafico graf = new Grafico()
                {
                    ID_MENUAPP = nidMenuAPP,
                    sdsTitulo = inMenuAPP.DS_MENU,
                    sdsTipoGrafico = inMenuAPP.FL_TIPOGRAFICO
                };

                CotacaoMoedaController cotac = new CotacaoMoedaController();
                int ncdMoeda = int.Parse(args[0]);
                var lstCotac = cotac.GetCotacao(ncdMoeda);

                List<Double> lstData = new List<Double>();

                foreach (MoedaCotacao cotacMoeda in lstCotac)
                {
                    lstData.Add(cotacMoeda.VL_COTACAO);
                }

                graf.Dados = new List<DadosGrafico>();

                graf.Dados.Add(new DadosGrafico()
                {
                    Entries = new Models.Entry() { Value = lstData },
                    Label = "Cotação Moeda",
                    cor = new CorGraf(142,94,162,200)// Color.FromArgb(200, 142, 94, 162)
                });

                List<string> lstLegenda = new List<string>();
                foreach (MoedaCotacao cotacMoeda in lstCotac)
                {
                    lstLegenda.Add(cotacMoeda.DT_COTACAO.Day + "/" + cotacMoeda.DT_COTACAO.Month);
                }
                graf.Legendas = lstLegenda;

                return graf;
            }
            catch { throw; }
        }

        #endregion
    }
}

