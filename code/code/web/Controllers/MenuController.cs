using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAppRoma.Menu;
using WebAppRoma.Models;

namespace WebAppRoma.Controllers
{
    [Authorize]
    [RoutePrefix("api/menu")]
    public class MenuController : ApiController
    {
        static string sdsUriWS = System.Configuration.ConfigurationManager.AppSettings["ida:Audience"];
        string uriImagem = sdsUriWS + "site/img/";
        string uriGrafico = sdsUriWS + "site/grafico/grafico";
        
        [HttpGet]
        [Route("GetMenu")]
        [AllowAnonymous]
        public List<GroupItem> GetMenu(string sdsEmail)
        {
            DbDataReader qryMenu = null;
            DbDataReader qrItMenu = null;

            List<GroupItem> lstMenu = null;

            Conexao con = Conexao.Instance(sdsEmail);
            try
            {
                var usuario = UsuarioEmailController.getUsuario(sdsEmail);
                var func = FuncionarioController.getFuncionario(usuario);

                string sdsSql = "select * from IN_GRPMENUAPP where (ID_GRPMENUAPP in(select ID_GRPMENUAPP from IN_MENUAPP where ID_MENUAPP in(select ID_MENUAPP from IN_MENUAPPCARGO where CD_CARGO = '" + func.CD_CARGO + "') and DT_INATIVO is null) or NR_SEQUEN in(1,99)) order by NR_SEQUEN";
                
                qryMenu = con.execQuery(sdsSql);

                if (qryMenu.HasRows)
                {
                    lstMenu = new List<GroupItem>();

                    while (qryMenu.Read())
                    {
                        var grp = new GroupItem() {
                            GroupName = qryMenu.GetString(qryMenu.GetOrdinal("DS_GRPMENU"))
                        };

                        if(qrItMenu != null)
                        {
                            if (!qrItMenu.IsClosed) qrItMenu.Close();
                        }

                        string sdsSqlFiltro = "";
                        var nidGrpMenu = Convert.ToInt64(qryMenu.GetValue(qryMenu.GetOrdinal("ID_GRPMENUAPP")));
                        if (nidGrpMenu != 3 && nidGrpMenu != 4)
                        {
                            sdsSqlFiltro = "ID_MENUAPP in(select ID_MENUAPP from IN_MENUAPPCARGO where CD_CARGO = '" + func.CD_CARGO + "' and DT_INATIVO is null) and ";
                        }

                        var sdsSqlIt = "select * from in_menuapp where DT_INATIVO is null and " + sdsSqlFiltro +
                            " ID_GRPMENUAPP = " +nidGrpMenu+ " and (level = 3 or "+
                            " (level = 2 and ID_MENUAPPSUP not in(select ID_MENUAPP from IN_MENUAPP where ID_MENUAPPSUP is not null)) or " +
                            " (level = 1 and id_menuappsup is null)) connect by prior ID_MENUAPP = ID_MENUAPPSUP order siblings by nr_sequen";
                        qrItMenu = con.execQuery(sdsSqlIt);

                        if (qrItMenu.HasRows)
                        {
                            List<ItemMenu> lstItem = new List<ItemMenu>();
                            ItemMenu _itemMenu = null;

                            while (qrItMenu.Read())
                            {
                                
                                string icone = "";
                                string classe = "";
                                bool bboGrafico = false;

                                if (!qrItMenu.IsDBNull(qrItMenu.GetOrdinal("DS_CLASSE"))) classe = qrItMenu.GetString(qrItMenu.GetOrdinal("DS_CLASSE"));
                                if (!qrItMenu.IsDBNull(qrItMenu.GetOrdinal("DS_ICONE"))) icone = uriImagem+qrItMenu.GetString(qrItMenu.GetOrdinal("DS_ICONE"));
                                if (!qrItMenu.IsDBNull(qrItMenu.GetOrdinal("FL_GRAFICO")))
                                {
                                    bboGrafico = qrItMenu.GetString(qrItMenu.GetOrdinal("FL_GRAFICO")) == "T";
                                }

                                if (bboGrafico)
                                {
                                    int sflTipoGrafico = 0;
                                    if (!qrItMenu.IsDBNull(qrItMenu.GetOrdinal("FL_TIPOGRAFICO")))
                                        sflTipoGrafico = Convert.ToInt16(qrItMenu.GetValue(qrItMenu.GetOrdinal("FL_TIPOGRAFICO")));

                                    if (!qrItMenu.IsDBNull(qrItMenu.GetOrdinal("ID_MENUAPPSUP")))
                                    {
                                        if (Convert.ToInt32(qrItMenu.GetValue(qrItMenu.GetOrdinal("ID_MENUAPPSUP"))) == _itemMenu.nidMenu)
                                        {
                                            _itemMenu.lstGraficos.Add(new GraficoURL
                                            {
                                                DS_TITULO = qrItMenu.GetString(qrItMenu.GetOrdinal("DS_MENU")),
                                                DS_URL = qrItMenu.GetString(qrItMenu.GetOrdinal("DS_URL")),
                                                ID_MENUAPP = Convert.ToInt32(qrItMenu.GetValue(qrItMenu.GetOrdinal("ID_MENUAPP"))),
                                                FL_TIPOGRAFICO = sflTipoGrafico,
                                                lstGraficos = new List<GraficoURL>()
                                            });
                                        }
                                        else
                                        {
                                            _itemMenu.lstGraficos[_itemMenu.lstGraficos.Count - 1].lstGraficos.Add(new GraficoURL {
                                                DS_TITULO = qrItMenu.GetString(qrItMenu.GetOrdinal("DS_MENU")),
                                                DS_URL = qrItMenu.GetString(qrItMenu.GetOrdinal("DS_URL")),
                                                ID_MENUAPP = Convert.ToInt32(qrItMenu.GetValue(qrItMenu.GetOrdinal("ID_MENUAPP"))),
                                                FL_TIPOGRAFICO = sflTipoGrafico
                                            });

                                        }

                                    }
                                }
                                else
                                {
                                    _itemMenu = new ItemMenu
                                    {
                                        nidMenu = qrItMenu.GetInt32(qrItMenu.GetOrdinal("ID_MENUAPP")),
                                        Title = qrItMenu.GetString(qrItMenu.GetOrdinal("DS_MENU")),
                                        IconSource = icone,
                                        TargetType = classe,
                                        bboForm = qrItMenu.GetString(qrItMenu.GetOrdinal("BO_FORM")) == "T",
                                        lstGraficos = new List<GraficoURL>()                                        
                                    };

                                    lstItem.Add(_itemMenu);
                                }
                            }
                            grp.ItensMenu = lstItem;
                        }

                        lstMenu.Add(grp);                     
                    }
                    return lstMenu;
                }
                else
                {
                    return null;
                }               
            }
            catch { throw; }
            finally
            {
                if (!qryMenu.IsClosed) qryMenu.Close();
                if (!qrItMenu.IsClosed) qrItMenu.Close();
                con.fechaCon();
            }
        }

        [HttpGet]
        [Route("VerificaFavorito")]
        public bool VerificaFavorito(double IdMenuApp, string sdsEmail)
        {
            DbDataReader qryGrafUsu = null;
            Conexao con = Conexao.Instance(sdsEmail);
            try
            {   
                qryGrafUsu = con.execQuery("select * from IN_GRAFAPPUSU where ID_MENUAPP = " + IdMenuApp + " and DS_EMAIL = '" + sdsEmail + "' and BO_FAVORITO = 'T'");
                if (qryGrafUsu.HasRows) return true;
                return false;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (qryGrafUsu != null)
                    if (!qryGrafUsu.IsClosed) qryGrafUsu.Close();
                con.fechaCon();
            }
            
        }

        [HttpGet]
        [Route("FavGrafico")]
        public void FavGrafico(double IdMenuAPP, bool bboFav, string sdsEmail)
        {
            DbDataReader qryMenu = null;
            DbDataReader qryGrafUsu = null;

            string sboFav = "F";
            if (bboFav) sboFav = "T";

            Conexao con = Conexao.Instance(sdsEmail);
            try
            {                
                qryMenu = con.execQuery("select * from IN_MENUAPP where ID_MENUAPP = "+IdMenuAPP);
                if (qryMenu.HasRows)
                {
                    qryMenu.Read();
                    string sdsCommand = "";

                    qryGrafUsu = con.execQuery("select * from IN_GRAFAPPUSU where ID_MENUAPP = " + IdMenuAPP + " and DS_EMAIL = '" + sdsEmail + "'");
                    if (qryGrafUsu.HasRows)
                        sdsCommand = "update IN_GRAFAPPUSU set BO_FAVORITO='" + sboFav + "' where ID_MENUAPP = " + IdMenuAPP + " and DS_EMAIL = '" + sdsEmail + "'";
                    else
                    {
                        float nidProx = con.NextSequence("IN_GRAFAPPUSU");
                        sdsCommand = "insert into IN_GRAFAPPUSU values(" + nidProx + "," + IdMenuAPP + ",'" + sdsEmail + "','" + sboFav + "')";
                    }
                    con.ExecCommand(sdsCommand);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!qryMenu.IsClosed) qryMenu.Close();
                if (qryGrafUsu != null)
                    if (!qryGrafUsu.IsClosed) qryGrafUsu.Close();

                con.fechaCon();
            }
        }

        [HttpGet]
        [Route("GetInicio")]
        public List<Grafico> GetInicio(string sdsEmail)
        {
            List<Grafico> lstGraf = new List<Grafico>();

            DbDataReader qryGrafUsu = null;
            DbDataReader qryGrafico = null;
            Conexao con = Conexao.Instance(sdsEmail);
            try
            {
                var sdsSQl = "select * from IN_GRAFAPPUSU where DS_EMAIL = '" + sdsEmail + "' and BO_FAVORITO = 'T'";
                qryGrafUsu = con.execQuery(sdsSQl);

                if (qryGrafUsu.HasRows)
                {
                    while (qryGrafUsu.Read())
                    {
                        sdsSQl = "select * from IN_MENUAPP where ID_MENUAPP = " + qryGrafUsu.GetInt32(qryGrafUsu.GetOrdinal("ID_MENUAPP"));
                        if (qryGrafico != null)
                            if (!qryGrafico.IsClosed) qryGrafico.Close();
                        qryGrafico = con.execQuery(sdsSQl);
                        qryGrafico.Read();

                        string sdsUrl = qryGrafico.GetString(qryGrafico.GetOrdinal("DS_URL"));
                        double nidMenuAPP = Convert.ToInt32(qryGrafico.GetValue(qryGrafico.GetOrdinal("ID_MENUAPP")));

                        int nnrPosBarra = sdsUrl.IndexOf("/");
                        int nnrPosParam = sdsUrl.IndexOf("?");
                        string sdsMetodo = sdsUrl.Substring(nnrPosBarra + 1, nnrPosParam - nnrPosBarra - 1);
                        string sdsParams = sdsUrl.Substring(nnrPosParam + 1, sdsUrl.Length - nnrPosParam - 1);

                        sdsParams = sdsParams.Replace("[MES]", DateTime.Now.Month.ToString()).Replace("[ANO]", DateTime.Now.Year.ToString());

                        List<String> args = new List<String>();
                        string[] sdsAux = sdsParams.Split(',');
                        foreach (string valor in sdsAux)
                        {
                            args.Add(valor);
                        }

                        List<DadosGrafico> dados = new List<DadosGrafico>();
                        GraficoDadosController grafDados = new GraficoDadosController();

                        switch (sdsMetodo)
                        {
                            case "GetGrafCarteira":
                                lstGraf.Add(grafDados.GetGrafCarteira(args, nidMenuAPP));
                                break;
                            case "GetGrafCarteiraGlobal":
                                lstGraf.Add(grafDados.GetGrafCarteiraGlobal(args, nidMenuAPP));
                                break;
                            case "GetFaturadoAnual":
                                lstGraf.Add(grafDados.GetFaturadoAnual(args, nidMenuAPP));
                                break;
                            case "GetFaturadoMercado":
                                lstGraf.Add(grafDados.GetFaturadoMercado(args, nidMenuAPP));
                                break;
                            case "GetAtingimentoMeta":
                                lstGraf.Add(grafDados.GetAtingimentoMeta(args, nidMenuAPP));
                                break;
                            case "GetCotacaoMoeda":
                                lstGraf.Add(grafDados.GetCotacaoMoeda(args, nidMenuAPP));
                                break;
                            case "GetPedidosEmitidosMes":
                                lstGraf.Add(grafDados.GetPedidosEmitidosMes(args, nidMenuAPP));
                                break;
                            case "GetPedidosCarteira":
                                lstGraf.Add(grafDados.GetPedidosCarteira(args, nidMenuAPP));
                                break;
                            case "GetPedidosEmitidosAno":
                                lstGraf.Add(grafDados.GetPedidosEmitidosAno(args, nidMenuAPP));
                                break;
                            default:
                                break;
                        }   
                    }
                }

                return lstGraf;
            }
            catch { return null; }
            finally
            {
                if (!qryGrafUsu.IsClosed) qryGrafUsu.Close();
                if(qryGrafico != null)
                    if (!qryGrafico.IsClosed) qryGrafico.Close();

                con.fechaCon();
            }
        }
    }
}
