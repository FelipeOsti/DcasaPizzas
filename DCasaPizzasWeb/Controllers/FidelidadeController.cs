using DCasaPizzasWeb.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DCasaPizzasWeb.Controllers
{
    [RoutePrefix("API/Fidelidade")]
    public class FidelidadeController : ApiController
    {

        [Route("GetPontos")]
        [HttpGet]
        public double GetPontos(string sdsParam)
        {

            if (sdsParam == "" || sdsParam == null) return 0;

            UsuarioController user = new UsuarioController();
            AtualizarPontos(user.GetIDUsuario(sdsParam));

            Conexao con = new Conexao();
            SqlDataReader pontos = null;
            try
            {                
                pontos = con.ExecQuery("select * from solari.IN_PONTOSUSUARIO where ID_USUARIO in(select ID_USUARIO from solari.in_usuario where DS_EMAIL = '" + sdsParam + "' or DS_CPF = '" + sdsParam + "')");
                if (pontos.HasRows)
                {
                    pontos.Read();
                    return Convert.ToInt64(pontos.GetValue(pontos.GetOrdinal("NR_PONTOS")));
                }
                else
                    return 0;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pontos != null)
                    if (!pontos.IsClosed) pontos.Close();

                con.FechaConexao();
            }
        }

        [Route("AcumulaPontos")]
        [HttpGet]
        public void AcumulaPontos(string sdsParam, int nnrPontos)
        {
            if (sdsParam == "" || sdsParam == null) return;

            Conexao con = new Conexao();
            SqlDataReader usuar = null;
            SqlDataReader pontos = null;
            double nidUsuar = 0;
            try
            {
                
                usuar = con.ExecQuery("select * from solari.in_usuario where DS_EMAIL = '" + sdsParam + "' or DS_CPF = '" + sdsParam + "'");
                if (usuar.HasRows)
                {
                    usuar.Read();
                    nidUsuar = Convert.ToInt64(usuar.GetValue(usuar.GetOrdinal("ID_USUARIO")));
                }
                else
                    throw new Exception("Usuário não encontrado!");

                string sdsSql = "";

                pontos = con.ExecQuery("select * from solari.IN_PONTOSUSUARIO where ID_USUARIO = " + nidUsuar);

                sdsSql = "insert into solari.IN_PONTOSUSUARIO values("+nidUsuar + "," + nnrPontos + ")";
                if (pontos.HasRows) sdsSql = "update solari.IN_PONTOSUSUARIO set NR_PONTOS = NR_PONTOS + " + nnrPontos+" where ID_USUARIO = "+nidUsuar;
                con.ExecCommand(sdsSql);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (usuar != null)
                    if (!usuar.IsClosed) usuar.Close();

                if (pontos != null)
                    if (!pontos.IsClosed) pontos.Close();

                con.FechaConexao();
            }
        }

        [Route("GerarToken")]
        [HttpPost]
        public string GerarToken(List<Pedido> pedido, string token, int nnrPontos)
        {
            var con = new Conexao();
            try
            {
                var IdPedido = pedido[0].ID_PEDIDO;
                string NrToken = "";
                /*string sdsAux = IdPedido.ToString();

                int x = 2;
                foreach (var N in sdsAux)
                {
                    double y = double.Parse(N.ToString());
                    NrToken = NrToken + (y * x);                
                    x+=2;
                }
                double token = double.Parse(NrToken) / DateTime.Now.Second;*/

                NrToken = token; // Math.Round(token).ToString();

                DateTime data = DateTime.Now;
                data = data.AddMonths(10);
                string sdsValidade = data.Year + "-" + data.Month + "-" + data.Day;

                var pedidoC = new PedidoController();
                var nidPedido = pedidoC.GravarPedido(pedido);
                if (nidPedido == 0) throw new Exception("Erro ao gerar pedido!");
                
                con.ExecCommand("insert into solari.in_token values('" + NrToken + "','" + sdsValidade + "',null," + nidPedido + ","+nnrPontos+ ",GETDATE())");

                return NrToken;
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

        internal void AtualizarPontos(long v)
        {
            SqlDataReader pontos = null;
            SqlDataReader trocas = null;
            var con = new Conexao();
            try
            {
                pontos = con.ExecQuery("select * from solari.IN_TOKEN where ID_PEDIDO in(select ID_PEDIDO from solari.PE_PEDIDO where ID_USUARIO = " + v + ")");
                if (pontos.HasRows)
                {
                    long nnrPontos = 0;

                    while(pontos.Read())
                    {
                        DateTime ddtValidade = pontos.GetDateTime(pontos.GetOrdinal("DT_VALIDADE"));                        
                        if(ddtValidade >= DateTime.Now)
                        {
                            nnrPontos += Convert.ToInt64(pontos.GetValue(pontos.GetOrdinal("NR_PONTOS")));
                        }
                    }

                    trocas = con.ExecQuery("select * from solari.PE_TROCA where DT_CANCELA is null and ID_USUARIO = " + v);
                    if(trocas.HasRows)
                    {
                        while (trocas.Read())
                        {
                            nnrPontos -= Convert.ToInt64(trocas.GetValue(trocas.GetOrdinal("NR_PONTOS")));
                        }
                    }

                    con.ExecCommand("update solari.IN_PONTOSUSUARIO set NR_PONTOS = " + nnrPontos + " where ID_USUARIO = " + v);
                }
            }            
            catch
            {
                return;
            }
            finally
            {
                if (pontos != null)
                    if (!pontos.IsClosed) pontos.Close();
                if (trocas != null)
                    if (!trocas.IsClosed) trocas.Close();
                con.FechaConexao();
            }
        }

        [Route("ValidarToken")]
        [HttpGet]
        public string ValidarToken(string NrToken, double IdUsuario)
        {
            var con = new Conexao();
            SqlDataReader token = null;
            try
            {
                
                token = con.ExecQuery("select * from solari.IN_TOKEN where NR_TOKEN = '" + NrToken + "'");
                if (token.HasRows)
                {
                    token.Read();
                    DateTime ddtValidade = token.GetDateTime(token.GetOrdinal("DT_VALIDADE"));
                    if (DateTime.Now.Date > ddtValidade.Date) return "V"; //Validade expirada
                    if (!token.IsDBNull(token.GetOrdinal("DT_UTILIZACAO"))) return "U"; //Já utilizado

                    double IdPedido = Convert.ToInt64(token.GetValue(token.GetOrdinal("ID_PEDIDO")));
                    double IdToken = Convert.ToInt64(token.GetValue(token.GetOrdinal("ID_TOKEN")));

                    con.ExecCommand("update solari.pe_pedido set ID_USUARIO = " + Math.Round(IdUsuario).ToString() + " where ID_PEDIDO = " + IdPedido);
                    con.ExecCommand("update solari.in_token set DT_UTILIZACAO = GETDATE() where ID_TOKEN = " + IdToken);

                    AcumulaPontosPedido(IdUsuario, Convert.ToInt32(token.GetValue(token.GetOrdinal("NR_PONTOS"))));

                    return "T";
                }

                return "F"; //Não encontrado.
            }
            catch
            {
                throw;
            }
            finally
            {
                if (token != null)
                    if (!token.IsClosed) token.Close();

                con.FechaConexao();
            }
        }

        [Route("ListarPontos")]
        [HttpGet]
        public List<Token> ListarPontos(double idUsuario)
        {
            SqlDataReader token = null;

            var lstRetorno = new List<Token>();
            var con = new Conexao();
            try
            {
                
                token = con.ExecQuery("select * from solari.IN_TOKEN where ID_PEDIDO in(select ID_PEDIDO from solari.PE_PEDIDO where ID_USUARIO = " + idUsuario+") order by DT_CADASTRO");
                if (token.HasRows)
                {
                    while (token.Read())
                    {
                        DateTime ddtValidade = token.GetDateTime(token.GetOrdinal("DT_VALIDADE"));
                        DateTime dataCompra = token.GetDateTime(token.GetOrdinal("DT_CADASTRO"));
                        int nnrPontos = Convert.ToInt32(token.GetValue(token.GetOrdinal("NR_PONTOS")));
                        DateTime ddtUtilizado = token.GetDateTime(token.GetOrdinal("DT_UTILIZACAO"));

                        lstRetorno.Add(new Token { DataCompra = dataCompra, DataTroca = ddtUtilizado, DataValidade = ddtValidade, NrPontos = nnrPontos });
                    }
                }

                return lstRetorno;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (token != null)
                    if (!token.IsClosed) token.Close();

                con.FechaConexao();
            }
        }

        [Route("ListarPontosExpirando")]
        [HttpGet]
        public List<Token> ListarPontosExpirando(double idUsuario)
        {
            SqlDataReader token = null;

            var lstRetorno = new List<Token>();
            var con = new Conexao();
            try
            {
                
                token = con.ExecQuery("select * from solari.IN_TOKEN where ID_PEDIDO in(select ID_PEDIDO from solari.PE_PEDIDO where ID_USUARIO = " + idUsuario + ") and DT_VALIDADE <= DateAdd(month, +1, GetDate()) order by DT_VALIDADE");
                if (token.HasRows)
                {
                    while (token.Read())
                    {
                        DateTime ddtValidade = token.GetDateTime(token.GetOrdinal("DT_VALIDADE"));
                        DateTime dataCompra = token.GetDateTime(token.GetOrdinal("DT_CADASTRO"));
                        int nnrPontos = Convert.ToInt32(token.GetValue(token.GetOrdinal("NR_PONTOS")));
                        DateTime ddtUtilizado = token.GetDateTime(token.GetOrdinal("DT_UTILIZACAO"));

                        lstRetorno.Add(new Token { DataCompra = dataCompra, DataTroca = ddtUtilizado, DataValidade = ddtValidade, NrPontos = nnrPontos });
                    }
                }

                return lstRetorno;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (token != null)
                    if (!token.IsClosed) token.Close();

                con.FechaConexao();
            }
        }

        [Route("GravarTroca")]
        [HttpPost]
        public string GravarTroca(List<Produto> lstProd, double idUsuario, double nrPontos)
        {
            var con = new Conexao();
            try
            {
                UsuarioController user = new UsuarioController();
                string email = user.GetEmail(idUsuario);

                var nnrPontosUsuar = GetPontos(email);
                if (nnrPontosUsuar < nrPontos) return "F";

                string sdsSqlTroca = "insert into solari.PE_TROCA values(GETDATE()," + idUsuario+"," + nrPontos + ",null,null)";
                con.ExecCommand(sdsSqlTroca);

                string sdsSql = "select max(ID_TROCA) as ID from solari.PE_TROCA where ID_USUARIO = " + idUsuario;
                var qryTroca = con.ExecQuery(sdsSql);

                double nid = 1;

                if (qryTroca.HasRows)
                {
                    qryTroca.Read();
                    nid = Convert.ToInt64(qryTroca.GetValue(qryTroca.GetOrdinal("ID")));
                }

                foreach (var prod in lstProd)
                {
                    string sdsSqlItTroca = "insert into solari.PE_ITTROCA values(" + nid + ","+prod.ID_PRODUTO+","+prod.NR_PONTOS+")";
                    con.ExecCommand(sdsSqlItTroca);
                }

                qryTroca.Close();

                AtualizaPontuacao(idUsuario, nrPontos, "-");

                return "T";
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                con.FechaConexao();
            }
        }

        private void AtualizaPontuacao(double idUsuario, double nrPontos, string sdsSinal)
        {
            var con = new Conexao();
            try
            {
                string sdsSqlTroca = "update solari.IN_PONTOSUSUARIO set NR_PONTOS = NR_PONTOS " + sdsSinal+" "+ nrPontos + " where ID_USUARIO = " + idUsuario;
                con.ExecCommand(sdsSqlTroca);
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

        [Route("ConfirmaTroca")]
        [HttpGet]
        public string ConfirmaTroca(long nidTroca)
        {
            Conexao con = new Conexao();
            try
            {
                con.ExecCommand("update solari.PE_TROCA set DT_CONFIRMA = GETDATE() where ID_TROCA = " + nidTroca);               
                return "OK";
            }
            catch
            {
                return "ERRO";
            }
            finally
            {
                con.FechaConexao();
            }
        }

        [Route("CancelarTroca")]
        [HttpGet]
        public string CancelarTroca(long nidTroca)
        {
            SqlDataReader trocas = null;
            Conexao con = new Conexao();
            try
            {
                con.ExecCommand("update solari.PE_TROCA set DT_CANCELA = GETDATE() where ID_TROCA = " + nidTroca);

                trocas = con.ExecQuery("select * from solari.PE_TROCA where ID_TROCA = " + nidTroca);
                if (trocas.HasRows)
                {
                    trocas.Read();

                    long nidUsuario = Convert.ToInt64(trocas.GetValue(trocas.GetOrdinal("ID_USUARIO")));
                    long nnrPontos = Convert.ToInt64(trocas.GetValue(trocas.GetOrdinal("NR_PONTOS")));

                    AtualizaPontuacao(nidUsuario, nnrPontos, "+");
                }

                return "OK";
            }
            catch
            {
                return "ERRO";
            }
            finally
            {
                if(trocas != null)
                    if (!trocas.IsClosed) trocas.Close();
                con.FechaConexao();
            }
        }

        [Route("GetTroca")]
        [HttpGet]
        public List<Trocas> GetTroca(int nidUsuario)
        {
            SqlDataReader trocas = null;
            SqlDataReader ittrocas = null;

            List<Trocas> lstTroca = new List<Trocas>();
            Conexao con = new Conexao();
            try
            {

                trocas = con.ExecQuery("select * from solari.PE_TROCA where ID_USUARIO = " + nidUsuario);
                if (trocas.HasRows)
                {
                    while (trocas.Read())
                    {
                        DateTime ddtConfirma = new DateTime(0);
                        if (!trocas.IsDBNull(trocas.GetOrdinal("DT_CONFIRMA"))) ddtConfirma = trocas.GetDateTime(trocas.GetOrdinal("DT_CONFIRMA"));

                        DateTime ddtCancela = new DateTime(0);
                        if (!trocas.IsDBNull(trocas.GetOrdinal("DT_CANCELA"))) ddtCancela = trocas.GetDateTime(trocas.GetOrdinal("DT_CANCELA"));

                        lstTroca.Add(new Trocas()
                        {
                            ID_USUARIO = Convert.ToInt64(trocas.GetValue(trocas.GetOrdinal("ID_USUARIO"))),
                            NR_PONTOS = Convert.ToInt64(trocas.GetValue(trocas.GetOrdinal("NR_PONTOS"))),
                            DT_TROCA = trocas.GetDateTime(trocas.GetOrdinal("DT_TROCA")),
                            ID_TROCA = Convert.ToInt64(trocas.GetValue(trocas.GetOrdinal("ID_TROCA"))),
                            DT_CONFIRMA = ddtConfirma,
                            DT_CANCELA = ddtCancela,
                            itensTroca = new List<ItemTroca>()
                        });

                        double nidTroca = Convert.ToInt64(trocas.GetValue(trocas.GetOrdinal("ID_TROCA")));

                        if (ittrocas != null)
                        {
                            if (!ittrocas.IsClosed)
                            {
                                ittrocas.Close();
                                con.FechaUltimaConexao();
                            }
                        }
                        ittrocas = con.ExecQuery("select * from solari.PE_ITTROCA where ID_TROCA = " + nidTroca);
                        if (ittrocas.HasRows)
                        {
                            while (ittrocas.Read())
                            {
                                lstTroca[lstTroca.Count - 1].itensTroca.Add(new ItemTroca()
                                {
                                    ID_TROCA = lstTroca[lstTroca.Count - 1].ID_TROCA,
                                    ID_ITEMTROCA = Convert.ToInt64(ittrocas.GetValue(ittrocas.GetOrdinal("ID_ITTROCA"))),
                                    ID_PRODUTO = Convert.ToInt64(ittrocas.GetValue(ittrocas.GetOrdinal("ID_PRODUTO"))),
                                    NR_PONTOS = Convert.ToInt64(ittrocas.GetValue(ittrocas.GetOrdinal("NR_PONTOS")))
                                });
                            }
                        }
                    }
                }

                return lstTroca;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (trocas != null)
                    if (!trocas.IsClosed) trocas.Close();
                if (ittrocas != null)
                    if (!ittrocas.IsClosed) ittrocas.Close();

                con.FechaConexao();
            }
        }

        [Route("RecuperarTrocas")]
        [HttpGet]
        public List<Trocas> RecuperarTrocas(string bboPendente)
        {
            SqlDataReader trocas = null;
            SqlDataReader ittrocas = null;

            List<Trocas> lstTroca = new List<Trocas>();
            Conexao con = new Conexao();
            try
            {
                
                if(bboPendente == "T") //Somente os pendentes
                    trocas = con.ExecQuery("select * from solari.PE_TROCA where DT_CONFIRMA is null and DT_CANCELA is null");
                else //Todos
                    trocas = con.ExecQuery("select * from solari.PE_TROCA");
                if (trocas.HasRows)
                {
                    while (trocas.Read())
                    {
                        DateTime ddtConfirma = new DateTime(0);
                        if (!trocas.IsDBNull(trocas.GetOrdinal("DT_CONFIRMA"))) ddtConfirma = trocas.GetDateTime(trocas.GetOrdinal("DT_CONFIRMA"));

                        DateTime ddtCancela = new DateTime(0);
                        if (!trocas.IsDBNull(trocas.GetOrdinal("DT_CANCELA"))) ddtConfirma = trocas.GetDateTime(trocas.GetOrdinal("DT_CANCELA"));

                        lstTroca.Add(new Trocas()
                        {
                            ID_USUARIO = Convert.ToInt64(trocas.GetValue(trocas.GetOrdinal("ID_USUARIO"))),
                            NR_PONTOS = Convert.ToInt64(trocas.GetValue(trocas.GetOrdinal("NR_PONTOS"))),
                            DT_TROCA = trocas.GetDateTime(trocas.GetOrdinal("DT_TROCA")),
                            ID_TROCA = Convert.ToInt64(trocas.GetValue(trocas.GetOrdinal("ID_TROCA"))),
                            DT_CONFIRMA = ddtConfirma,
                            DT_CANCELA = ddtCancela,
                            itensTroca = new List<ItemTroca>()
                        });

                        double nidTroca = Convert.ToInt64(trocas.GetValue(trocas.GetOrdinal("ID_TROCA")));

                        if (ittrocas != null)
                        {
                            if (!ittrocas.IsClosed)
                            {
                                ittrocas.Close();
                                con.FechaUltimaConexao();
                            }
                        }
                        ittrocas = con.ExecQuery("select * from solari.PE_ITTROCA where ID_TROCA = " + nidTroca);
                        if (ittrocas.HasRows)
                        {
                            while (ittrocas.Read())
                            {
                                lstTroca[lstTroca.Count - 1].itensTroca.Add(new ItemTroca(){
                                    ID_ITEMTROCA = Convert.ToInt64(ittrocas.GetValue(ittrocas.GetOrdinal("ID_ITTROCA"))),
                                    ID_PRODUTO = Convert.ToInt64(ittrocas.GetValue(ittrocas.GetOrdinal("ID_PRODUTO"))),
                                    NR_PONTOS = Convert.ToInt64(ittrocas.GetValue(ittrocas.GetOrdinal("NR_PONTOS")))
                                });
                            }
                        }
                    }
                }

                return lstTroca;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (trocas != null)
                    if (!trocas.IsClosed) trocas.Close();
                if (ittrocas != null)
                    if (!ittrocas.IsClosed) ittrocas.Close();

                con.FechaConexao();
            }
        }

        private void AcumulaPontosPedido(double idUsuario, int v)
        {
            Conexao con = new Conexao();
            SqlDataReader usuario = null;
            try
            {
                
                usuario = con.ExecQuery("select * from solari.IN_USUARIO where ID_USUARIO = " + idUsuario);
                usuario.Read();
                AcumulaPontos(usuario.GetString(usuario.GetOrdinal("DS_EMAIL")), v);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (usuario != null)
                    if (!usuario.IsClosed) usuario.Close();

                con.FechaConexao();
            }
        }
    }
}
