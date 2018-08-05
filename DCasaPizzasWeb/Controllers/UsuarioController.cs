using DCasaPizzasWeb.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace DCasaPizzasWeb.Controllers
{
    [RoutePrefix("API/Usuario")]
    public class UsuarioController : ApiController
    {
        [Route("GetIDUsuario")]
        [HttpGet]
        public long GetIDUsuario(string sdsEmail)
        {
            SqlDataReader user = null;
            Conexao con = new Conexao();
            try
            {                
                user = con.ExecQuery("select * from solari.IN_USUARIO where DS_EMAIL = '" + sdsEmail + "'");
                if (user.HasRows)
                {
                    user.Read();
                    return Convert.ToInt64(user.GetValue(user.GetOrdinal("ID_USUARIO")));
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
                if (user != null)
                    if (!user.IsClosed) user.Close();

                con.FechaConexao();
            }
        }

        [HttpGet]
        [Route("CriptografaSHA256")]
        public string CriptografaSHA256(string senha)
        {
            SHA256Managed crypt = new SHA256Managed();
            StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(senha), 0, Encoding.UTF8.GetByteCount(senha));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            var retorno = hash.ToString();
            return retorno.TrimStart('"').TrimEnd('"');
        }

        [Route("VerificaUsuarioExiste")]
        [HttpGet]
        public string VerificaUsuarioExiste(string sdsEmail)
        {
            SqlDataReader user = null;
            Conexao con = new Conexao();
            try
            {                
                user = con.ExecQuery("select * from solari.IN_USUARIO where DS_EMAIL = '" + sdsEmail + "'");
                if (user.HasRows)
                {
                    return "T";
                }

                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                if (user != null)
                    if (!user.IsClosed) user.Close();

                con.FechaConexao();
            }
        }

        [Route("VerificaUsuarioSenha")]
        [HttpGet]
        public string VerificaUsuarioSenha(string sdsEmail, string sdsSenha)
        {
            Conexao con = new Conexao();
            SqlDataReader user = null;
            try
            {
                user = con.ExecQuery("select * from solari.IN_USUARIO where DS_EMAIL = '" + sdsEmail + "' and DS_SENHA = '"+sdsSenha+"'");
                if (user.HasRows)
                {
                    user.Read();
                    return "T";
                }

                return "F";
            }
            catch
            {
                throw;
            }
            finally
            {
                if (user != null)
                    if (!user.IsClosed) user.Close();


                con.FechaConexao();
            }
        }

        [Route("GetNome")]
        [HttpGet]
        public string GetNome(string sdsEmail)
        {
            SqlDataReader user = null;
            Conexao con = new Conexao();
            try
            {
                
                user = con.ExecQuery("select * from solari.IN_USUARIO where DS_EMAIL = '" + sdsEmail + "'");
                if (user.HasRows)
                {
                    user.Read();
                    return user.GetString(user.GetOrdinal("NM_NOME"));
                }

                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                if (user != null)
                    if (!user.IsClosed) user.Close();
                con.FechaConexao();
            }
        }

        internal string GetNomeID(double idUsuario)
        {
            SqlDataReader user = null;
            Conexao con = new Conexao();
            try
            {
                
                user = con.ExecQuery("select * from solari.IN_USUARIO where ID_USUARIO = " + idUsuario);
                if (user.HasRows)
                {
                    user.Read();
                    return user.GetString(user.GetOrdinal("NM_NOME"));
                }

                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                if (user != null)
                    if (!user.IsClosed) user.Close();

                con.FechaConexao();
            }
        }

        [Route("NovoUsuario")]
        [HttpPost]
        public bool NovoUsuario(UsuarioModel usuar)
        {
            SqlDataReader user = null;
            Conexao con = new Conexao();
            try
            {

                string sdsSql = "";

                user = con.ExecQuery("select * from solari.IN_USUARIO where DS_EMAIL = '" + usuar.DS_EMAIL + "' or (DS_CPF = '" + usuar.DS_CPF + "' and DS_CPF is not null and DS_CPF <> '')");
                if (user.HasRows)
                {
                    user.Read();                    
                    sdsSql = "update solari.IN_USUARIO set DS_SENHA='" + usuar.DS_SENHA + "',NM_NOME='" + usuar.NM_NOME + "',DS_TELEFONE='" + usuar.DS_TELEFONE + "',DS_ENDERECO='" + usuar.DS_ENDERECO + "',NR_NUMERO='" + usuar.NR_NUMERO + "',DS_BAIRRO='" + usuar.DS_BAIRRO + "',DS_COMPLEMENTO='" + usuar.DS_COMPLEMENTO + "', BO_FACEBOOK='"+usuar.BO_FACEBOOK+"', BO_SENHAPROV='F' where ID_USUARIO = "+Convert.ToInt32(user.GetValue(user.GetOrdinal("ID_USUARIO")));
                }
                else
                    sdsSql = "insert into solari.IN_USUARIO values ('" + usuar.DS_EMAIL + "','" + usuar.DS_CPF + "','" + usuar.DS_SENHA + "','" + usuar.NM_NOME + "','" + usuar.DS_TELEFONE + "','" + usuar.DS_ENDERECO + "','" + usuar.NR_NUMERO + "','" + usuar.DS_BAIRRO + "','" + usuar.DS_COMPLEMENTO + "','"+usuar.BO_FACEBOOK+"','F')";

                con.ExecCommand(sdsSql);

                return true;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (user != null)
                    if (!user.IsClosed) user.Close();

                con.FechaConexao();
            }
        }

        [Route("CriarSenha")]
        [HttpGet]
        public string CriarSenha(string sdsEmail)
        {

            try
            {
                string caracteresPermitidos = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
                char[] chars = new char[5];
                Random rd = new Random();
                for (int i = 0; i < 5; i++)
                {
                    chars[i] = caracteresPermitidos[rd.Next(0, caracteresPermitidos.Length)];
                }
                string senha = new string(chars);

                AtualizaSenha(sdsEmail, senha, true);

                EmailController emailC = new EmailController();

                string sdsConteudo = "Olá, Voce solicitou uma nova senha para o aplicativo D'Casa Pizzas: " + senha;
                string sdsTitulo = "Nova senha criada";

                emailC.EnviarEmail(sdsEmail, sdsTitulo, sdsConteudo);

                return "";
            }
            catch
            {
                throw;
            }            
        }

        internal void AtualizaSenha(string sdsEmail, string sdsSenha, bool bboProv = false)
        {
            var con = new Conexao();
            try
            {
                string senhaCripto = CriptografaSHA256(sdsSenha);

                string sboProv = "F";
                if (bboProv) sboProv = "T";

                con.ExecCommand("update solari.IN_USUARIO set DS_SENHA = '" + senhaCripto + "', BO_SENHAPROV = '" + sboProv + "' where DS_EMAIL = '" + sdsEmail + "'");
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

        [Route("SenhaProvisoria")]
        [HttpGet]
        public string SenhaProvisoria(string sdsEmail)
        {
            var con = new Conexao();
            SqlDataReader user = null;

            try
            {
                string sdsSql = @"SELECT BO_SENHAPROV FROM solari.in_usuario where DS_EMAIL = '" + sdsEmail + "'";
                user = con.ExecQuery(sdsSql);

                if (user.HasRows)
                {
                    user.Read();
                    if (user.IsDBNull(user.GetOrdinal("BO_SENHAPROV"))) return "F";
                    if (user.GetString(user.GetOrdinal("BO_SENHAPROV")) == "T") return "T";
                }

                return "F";
            }
            catch
            {
                throw;
            }
            finally
            {
                if (user != null)
                    if (!user.IsClosed) user.Close();

                con.FechaConexao();
            }
        }

        [Route("GetDadosUsuario")]
        [HttpGet]
        public UsuarioModel GetDadosUsuario(string sdsEmail)
        {
            var con = new Conexao();
            SqlDataReader user = null;

            try
            {
                string sdsSql = @"SELECT * FROM solari.in_usuario where DS_EMAIL = '"+sdsEmail+"'";
                user = con.ExecQuery(sdsSql);

                if (user.HasRows)
                {
                    user.Read();

                    return new UsuarioModel()
                    {
                        ID_USUARIO = Convert.ToInt64(user.GetValue(user.GetOrdinal("ID_USUARIO"))),
                        DS_EMAIL = user.GetString(user.GetOrdinal("DS_EMAIL")),
                        DS_CPF = user.IsDBNull(user.GetOrdinal("DS_CPF")) ? null : user.GetString(user.GetOrdinal("DS_CPF")),
                        NM_NOME = user.IsDBNull(user.GetOrdinal("NM_NOME")) ? null : user.GetString(user.GetOrdinal("NM_NOME")),
                        DS_TELEFONE = user.IsDBNull(user.GetOrdinal("DS_TELEFONE")) ? null : user.GetString(user.GetOrdinal("DS_TELEFONE")),
                        DS_ENDERECO = user.IsDBNull(user.GetOrdinal("DS_ENDERECO")) ? null : user.GetString(user.GetOrdinal("DS_ENDERECO")),
                        NR_NUMERO = user.IsDBNull(user.GetOrdinal("NR_NUMERO")) ? null : user.GetString(user.GetOrdinal("NR_NUMERO")),
                        DS_BAIRRO = user.IsDBNull(user.GetOrdinal("DS_BAIRRO")) ? null : user.GetString(user.GetOrdinal("DS_BAIRRO")),
                        DS_COMPLEMENTO = user.IsDBNull(user.GetOrdinal("DS_COMPLEMENTO")) ? null : user.GetString(user.GetOrdinal("DS_COMPLEMENTO")),
                        BO_FACEBOOK = user.IsDBNull(user.GetOrdinal("BO_FACEBOOK")) ? null : user.GetString(user.GetOrdinal("BO_FACEBOOK"))
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (user != null)
                    if (!user.IsClosed) user.Close();

                con.FechaConexao();
            }
        }


        [Route("GetClientesAPP")]
        [HttpGet]
        public List<ClientesAPP> GetClientesAPP()
        {
            var con = new Conexao();
            SqlDataReader user = null;

            List<ClientesAPP> clientes = new List<ClientesAPP>();
            try
            {
                string sdsSql = @"SELECT a.ID_USUARIO, a.DS_EMAIL, a.DS_CPF, a.NM_NOME, a.DS_TELEFONE, a.DS_ENDERECO, a.NR_NUMERO, a.DS_BAIRRO, a.DS_COMPLEMENTO,b.NR_PONTOS
 FROM solari.in_usuario a left join solari.in_pontosusuario b on a.ID_USUARIO = b.ID_USUARIO";
                user = con.ExecQuery(sdsSql);

                if (user.HasRows)
                {
                    while (user.Read())
                    {
                        long nnrPontos = 0;
                        if (!user.IsDBNull(user.GetOrdinal("NR_PONTOS")))
                            nnrPontos = Convert.ToInt64(user.GetValue(user.GetOrdinal("NR_PONTOS")));

                        clientes.Add(new ClientesAPP()
                        {
                            ID_USUARIO = Convert.ToInt64(user.GetValue(user.GetOrdinal("ID_USUARIO"))),
                            DS_EMAIL = user.GetString(user.GetOrdinal("DS_EMAIL")),
                            DS_CPF = user.IsDBNull(user.GetOrdinal("DS_CPF")) ? null : user.GetString(user.GetOrdinal("DS_CPF")),
                            NM_NOME = user.IsDBNull(user.GetOrdinal("NM_NOME")) ? null : user.GetString(user.GetOrdinal("NM_NOME")),
                            DS_TELEFONE = user.IsDBNull(user.GetOrdinal("DS_TELEFONE")) ? null : user.GetString(user.GetOrdinal("DS_TELEFONE")),
                            DS_ENDERECO = user.IsDBNull(user.GetOrdinal("DS_ENDERECO")) ? null : user.GetString(user.GetOrdinal("DS_ENDERECO")),
                            NR_NUMERO = user.IsDBNull(user.GetOrdinal("NR_NUMERO")) ? null : user.GetString(user.GetOrdinal("NR_NUMERO")),
                            DS_BAIRO = user.IsDBNull(user.GetOrdinal("DS_BAIRRO")) ? null : user.GetString(user.GetOrdinal("DS_BAIRRO")),
                            DS_COMPLEMENTO = user.IsDBNull(user.GetOrdinal("DS_COMPLEMENTO")) ? null : user.GetString(user.GetOrdinal("DS_COMPLEMENTO")),
                            NR_PONTOS = nnrPontos
                        });
                    }
                }

                return clientes;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (user != null)
                    if (!user.IsClosed) user.Close();

                con.FechaConexao();
            }
        }

        internal string GetEmail(double idUsuario)
        {
            SqlDataReader user = null;
            Conexao con = new Conexao();
            try
            {

                user = con.ExecQuery("select * from solari.IN_USUARIO where ID_USUARIO = " + idUsuario);
                if (user.HasRows)
                {
                    user.Read();
                    return user.GetString(user.GetOrdinal("DS_EMAIL"));
                }

                return "";
            }
            catch
            {
                throw;
            }
            finally
            {
                if (user != null)
                    if (!user.IsClosed) user.Close();

                con.FechaUltimaConexao();
            }
        }
    }
}
