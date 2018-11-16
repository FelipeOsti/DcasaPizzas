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
    [RoutePrefix("api/clienteinterno")]
    public class ClienteInternoController : ApiController
    {
        [HttpGet]
        [Route("VerificaUsuario/{usuario}/{senha}")]
        public long VerificaUsuario(string usuario, string senha)
        {
            var con = new Conexao();
            SqlDataReader cliente = null;
            try
            {
                cliente = con.ExecQuery("select * from solari.CM_CLIENTEINTERNO where CD_LOGIN = '" + usuario + "' and DS_SENHA = '" + senha + "'");
                if (!cliente.HasRows) return 0;
                cliente.Read();
                return Convert.ToInt64(cliente["ID_CLIENTEINTERNO"]);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (cliente != null)
                    if (!cliente.IsClosed) cliente.Close();
                con.FechaConexao();
            }
        }

        [HttpPost]
        [Route("SalvarCliente")]
        public void SalvarCliente(IN_CLIENTEINTERNOModel cli)
        {
            var con = new Conexao();
            try
            {
                var sdsSql = @"update solari.CM_CLIENTEINTERNO set 
CD_UF = '" + cli.CD_UF + @"',
DS_CIDADE = '" + cli.DS_CIDADE + @"',
DS_NOME = '" + cli.DS_NOME + @"',
DS_ENDERECO = '" + cli.DS_ENDERECO + @"',
NR_CEP = '" + cli.NR_CEP + @"',
NR_CPF = '" + cli.NR_CPF + @"',
NR_CNPJ = '" + cli.NR_CNPJ + @"',
NR_NUMERO = '" + cli.NR_NUMERO + @"',
DS_TELEFONE = '" + cli.DS_TELEFONE + @"',
DS_BAIRRO = '" + cli.DS_BAIRRO + @"',
DS_CELULAR = '" + cli.DS_CELULAR + @"' 
where ID_CLIENTEINTERNO = "+cli.ID_CLIENTEINTERNO;

                con.ExecCommand(sdsSql);
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

        [HttpGet]
        [Route("GetClienteInterno/{nidCliente}")]
        public IN_CLIENTEINTERNOModel GetClienteInterno(long nidCliente)
        {
            var con = new Conexao();
            SqlDataReader cliente = null;
            try
            {                
                cliente = con.ExecQuery("select * from solari.CM_CLIENTEINTERNO where ID_CLIENTEINTERNO = "+nidCliente);
                if (!cliente.HasRows) return new IN_CLIENTEINTERNOModel(); 
                cliente.Read();
                var retorno = new IN_CLIENTEINTERNOModel()
                {
                    CD_LOGIN = cliente["CD_LOGIN"] == DBNull.Value ? "" : (string)cliente["CD_LOGIN"],
                    CD_UF = cliente["CD_UF"] == DBNull.Value ? "" : (string)cliente["CD_UF"],
                    DS_CELULAR = cliente["DS_CELULAR"] == DBNull.Value ? "" : (string)cliente["DS_CELULAR"],
                    DS_CIDADE = cliente["DS_CIDADE"] == DBNull.Value ? "" : (string)cliente["DS_CIDADE"],
                    DS_ENDERECO = cliente["DS_ENDERECO"] == DBNull.Value ? "" : (string)cliente["DS_ENDERECO"],
                    DS_NOME = cliente["DS_NOME"] == DBNull.Value ? "" : (string)cliente["DS_NOME"],
                    DS_TELEFONE = cliente["DS_TELEFONE"] == DBNull.Value ? "" : (string)cliente["DS_TELEFONE"],
                    ID_CLIENTEINTERNO = nidCliente,
                    ID_PLANO = cliente["ID_PLANO"] == DBNull.Value ? 0 : Convert.ToInt64(cliente["ID_PLANO"]),
                    NR_CEP = cliente["NR_CEP"] == DBNull.Value ? "" : (string)cliente["NR_CEP"],
                    NR_CNPJ = cliente["NR_CNPJ"] == DBNull.Value ? "" : (string)cliente["NR_CNPJ"],
                    NR_CPF = cliente["NR_CPF"] == DBNull.Value ? "" : (string)cliente["NR_CPF"],
                    NR_NUMERO = cliente["NR_NUMERO"] == DBNull.Value ? "" : (string)cliente["NR_NUMERO"],
                    NR_PARCELASIMPLANT = cliente["NR_PARCELAS"] == DBNull.Value ? 0 : Convert.ToInt32(cliente["NR_PARCELAS"]),
                    VL_IMPLANTACAO = cliente["VL_IMPLANTACAO"] == DBNull.Value ? 0 : Convert.ToDouble(cliente["VL_IMPLANTACAO"]),
                    NR_DIAVENCIMENTO = cliente["NR_DIAVENCIMENTO"] == DBNull.Value ? 0 : Convert.ToInt32(cliente["NR_DIAVENCIMENTO"]),
                    BO_ADMIN = cliente["BO_ADMIN"] == DBNull.Value ? "" : (string)cliente["BO_ADMIN"],
                    DS_BAIRRO = cliente["DS_BAIRRO"] == DBNull.Value ? "" : (string)cliente["DS_BAIRRO"]
                };

                return retorno;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (cliente != null)
                    if (!cliente.IsClosed) cliente.Close();
                con.FechaConexao();
            }
        }
    }
}
