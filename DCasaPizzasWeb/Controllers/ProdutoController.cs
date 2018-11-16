using DCasaPizzasWeb.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DCasaPizzasWeb.Controllers
{
    [RoutePrefix("API/Produto")]
    public class ProdutoController : ApiController
    {

        [Route("GetProdutos")]
        [HttpGet]
        public List<Produto> GetProdutos()
        {
            SqlDataReader produto = null;
            try
            {
                List<Produto> lstProdutos = new List<Produto>();

                Conexao con = new Conexao();
                produto = con.ExecQuery("select * from solari.MT_PRODUTO");
                if (produto.HasRows)
                {
                    while (produto.Read())
                    {
                        double nidProduto = Convert.ToInt64(produto.GetValue(produto.GetOrdinal("ID_PRODUTO")));
                        string scdProduto = produto.GetString(produto.GetOrdinal("CD_PRODUTO"));
                        string sdsProduto = produto.GetString(produto.GetOrdinal("DS_PRODUTO"));
                        string sdsTamanho = produto.GetString(produto.GetOrdinal("DS_TAMANHO"));
                        int nnrPontos = Convert.ToInt32(produto.GetValue(produto.GetOrdinal("NR_PONTOS")));
                        double nvlValor = Convert.ToInt64(produto.GetValue(produto.GetOrdinal("VL_PRODUTO")));
                        string sdsCategoria = produto.GetString(produto.GetOrdinal("DS_CATEGORIA"));

                        lstProdutos.Add(new Produto()
                        {
                            ID_PRODUTO = nidProduto,
                            CD_PRODUTO = scdProduto,
                            DS_PRODUTO = sdsProduto,
                            DS_TAMANHO = sdsTamanho,
                            NR_PONTOS = nnrPontos,
                            VL_VALOR = nvlValor,
                            DS_CATEGORIA = sdsCategoria
                        });
                    }
                }

                return lstProdutos;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (produto != null)
                    if (!produto.IsClosed) produto.Close();
            }
        }

        
        internal Produto GetProduto(long nidProduto)
        {
            SqlDataReader produto = null;
            try
            {
                Produto retorno = null;

                Conexao con = new Conexao();
                produto = con.ExecQuery("select * from solari.MT_PRODUTO where ID_PRODUTO = " + nidProduto);
                if (produto.HasRows)
                {

                    produto.Read();
                    string scdProduto = produto.GetString(produto.GetOrdinal("CD_PRODUTO"));
                    string sdsProduto = produto.GetString(produto.GetOrdinal("DS_PRODUTO"));
                    string sdsTamanho = produto.GetString(produto.GetOrdinal("DS_TAMANHO"));
                    int nnrPontos = Convert.ToInt32(produto.GetValue(produto.GetOrdinal("NR_PONTOS")));
                    double nvlValor = Convert.ToInt64(produto.GetValue(produto.GetOrdinal("VL_PRODUTO")));
                    string sdsCategoria = produto.GetString(produto.GetOrdinal("DS_CATEGORIA"));

                    retorno = new Produto()
                    {
                        ID_PRODUTO = nidProduto,
                        CD_PRODUTO = scdProduto,
                        DS_PRODUTO = sdsProduto,
                        DS_TAMANHO = sdsTamanho,
                        NR_PONTOS = nnrPontos,
                        VL_VALOR = nvlValor,
                        DS_CATEGORIA = sdsCategoria
                    };
                   
                }
                else
                {
                    retorno = new Produto()
                    {
                        ID_PRODUTO = nidProduto,
                        CD_PRODUTO = "0",
                        DS_PRODUTO = "Nao encontrado",
                        DS_TAMANHO = "Unico",
                        NR_PONTOS = 0,
                        VL_VALOR = 0,
                        DS_CATEGORIA = ""
                    };
                }

                return retorno;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (produto != null)
                    if (!produto.IsClosed) produto.Close();
            }
        }

        [Route("ExcluirProduto")]
        [HttpPost]
        public string ExcluirProduto(string scdProduto, string sdsTamanho)
        {
            try
            {
                Conexao con = new Conexao();

                if (!con.ExecCommand("delete from MT_PRODUTO where CD_PRODUTO = '"+scdProduto+ "' and DS_TAMANHO = '" + sdsTamanho + "'"))
                {
                    throw new Exception("Erro ao excluir o produto");
                }

                return "T";
            }
            catch
            {
                throw;
            }
        }

        [Route("LimparProdutos")]
        [HttpPost]
        public string LimparProdutos()
        {
            try
            {
                Conexao con = new Conexao();

                if (!con.ExecCommand("delete from MT_PRODUTO"))
                {
                    throw new Exception("Erro ao excluir os produtos.");
                }
                           
                return "T";
            }
            catch
            {
                throw;
            }
        }

        [Route("SalvarProduto")]
        [HttpPost]
        public string SalvarProduto(List<Produto> lstProdutos)
        {
            Conexao con = new Conexao();
            SqlDataReader produto = null;
            try
            {
                

                foreach (Produto prod in lstProdutos)
                {
                    if (produto != null)
                        if (!produto.IsClosed) produto.Close();

                    produto = con.ExecQuery("select * from solari.MT_PRODUTO where CD_PRODUTO = '" + prod.CD_PRODUTO+ "' and DS_TAMANHO = '" + prod.DS_TAMANHO + "'");
                    if (produto.HasRows)
                    {
                        if (!con.ExecCommand("update solari.MT_PRODUTO set DS_CATEGORIA = '" + prod.DS_CATEGORIA + "', DS_PRODUTO = '" + prod.DS_PRODUTO + "', VL_PRODUTO = " + prod.VL_VALOR.ToString() + ", NR_PONTOS = " + prod.NR_PONTOS + " where CD_PRODUTO = '" + prod.CD_PRODUTO + "' and DS_TAMANHO = '" + prod.DS_TAMANHO + "'"))
                        {
                            throw new Exception("Erro ao atualizar o produto.");
                        }
                    }
                    else
                    {
                        if (!con.ExecCommand("insert into solari.MT_PRODUTO values ('" + prod.CD_PRODUTO + "','" + prod.DS_PRODUTO + "'," + prod.VL_VALOR.ToString() + "," + prod.NR_PONTOS + ",'" + prod.DS_TAMANHO + "','" + prod.DS_CATEGORIA + "')")) 
                        {
                            throw new Exception("Erro ao incluir o produto.");
                        }
                    }
                }

                return "T";
            }
            catch
            {
                return "F";
            }
            finally
            {
                if (produto != null)
                    if (!produto.IsClosed) produto.Close();

                con.FechaConexao();
            }
        }
    }
}
