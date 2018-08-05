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
   
    [RoutePrefix("API/Pedido")]
    public class PedidoController : ApiController
    {
        internal double GravarPedido(List<Pedido> pedidos)
        {
            SqlDataReader Qpedido = null;
            Conexao con = new Conexao();
            try
            {
                

                double nidPedido = 0;

                foreach (var pedido in pedidos)
                {
                    con.ExecCommand("insert into solari.PE_PEDIDO values(GETDATE(),null,'" + pedido.DS_CLIENTE + "'," + pedido.VL_PEDIDO + ")");

                    if (Qpedido != null) {
                        if (!Qpedido.IsClosed)
                        {
                            Qpedido.Close();
                            con.FechaUltimaConexao();
                        }
                    }

                    Qpedido = con.ExecQuery("select max(ID_PEDIDO) as ID_PEDIDO from solari.PE_PEDIDO where DS_CLIENTE = '" + pedido.DS_CLIENTE + "' and VL_PEDIDO = " + pedido.VL_PEDIDO);
                    Qpedido.Read();
                    nidPedido = Convert.ToInt64(Qpedido.GetValue(Qpedido.GetOrdinal("ID_PEDIDO")));

                    foreach (var item in pedido.itens)
                    {
                        con.ExecCommand("insert into solari.PE_ITEMPEDIDO values(" + nidPedido + "," + item.CD_PRODUTO + ",'" + item.DS_PRODUTO + "'," + item.QT_PRODUTO + "," + item.VL_UNITARIO + "," + item.VL_TOTAL + ")");
                    }                  
                }

                return nidPedido;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Qpedido != null)
                    if (!Qpedido.IsClosed) Qpedido.Close();

                con.FechaConexao();
            }
        }

        [HttpGet]
        [Route("ListarPedidos")]
        public List<Pedido> ListarPedidos(double IdUsuario)
        {
            SqlDataReader pedido = null;
            SqlDataReader itemPedido = null;
            Conexao con = new Conexao();
            try
            {
                List<Pedido> lstPedido = new List<Pedido>();
                             
                pedido = con.ExecQuery("select * from solari.PE_PEDIDO where ID_USUARIO = " + IdUsuario);
                if (pedido.HasRows)
                {

                    UsuarioController usuario = new UsuarioController();
                    string sdsNome = usuario.GetNomeID(IdUsuario);

                    while (pedido.Read())
                    {
                        double nidPedido = Convert.ToInt64(pedido.GetValue(pedido.GetOrdinal("ID_PEDIDO")));
                        DateTime ddtPedido = DateTime.Now;
                        if (!pedido.IsDBNull(pedido.GetOrdinal("DT_PEDIDO")))
                            ddtPedido = pedido.GetDateTime(pedido.GetOrdinal("DT_PEDIDO"));

                        double nvlPedido = Convert.ToInt64(pedido.GetValue(pedido.GetOrdinal("VL_PEDIDO")));

                        if (itemPedido != null)
                        {
                            if (!itemPedido.IsClosed)
                            {
                                itemPedido.Close();
                                con.FechaUltimaConexao();
                            }
                        }
                        itemPedido = con.ExecQuery("select * from solari.PE_ITEMPEDIDO where ID_PEDIDO = " + Convert.ToInt64(pedido.GetValue(pedido.GetOrdinal("ID_PEDIDO"))).ToString());

                        lstPedido.Add(new Pedido()
                        {
                            ID_PEDIDO = nidPedido,
                            DS_CLIENTE = sdsNome,
                            VL_PEDIDO = nvlPedido,
                            DT_PEDIDO = ddtPedido,
                            itens = new List<ItemPedido>()
                        });

                        if (itemPedido.HasRows)
                        {
                            while (itemPedido.Read())
                            {

                                string scdProduto = itemPedido.GetString(itemPedido.GetOrdinal("CD_PRODUTO"));
                                string sdsProduto = itemPedido.GetString(itemPedido.GetOrdinal("DS_PRODUTO"));
                                double nqtItem = Convert.ToInt64(itemPedido.GetValue(itemPedido.GetOrdinal("QT_PRODUTO")));
                                double nvlUnit = Convert.ToDouble(itemPedido.GetValue(itemPedido.GetOrdinal("VL_UNITARIO")));
                                double nvlTotal = Convert.ToDouble(itemPedido.GetValue(itemPedido.GetOrdinal("VL_TOTAL")));

                                
                                lstPedido[lstPedido.Count-1].itens.Add(new ItemPedido()
                                {                                    
                                    CD_PRODUTO = scdProduto,
                                    DS_PRODUTO = sdsProduto,
                                    QT_PRODUTO = nqtItem,
                                    VL_UNITARIO = nvlUnit,
                                    VL_TOTAL = nvlTotal
                                });
                            }
                        }
                    }
                }

               
                return lstPedido;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pedido != null)
                    if (!pedido.IsClosed) pedido.Close();

                if (itemPedido != null)
                    if (!itemPedido.IsClosed) itemPedido.Close();

                con.FechaConexao();
            }
        }
    }
}
