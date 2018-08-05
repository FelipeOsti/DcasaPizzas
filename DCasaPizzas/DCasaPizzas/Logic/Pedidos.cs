using DCasaPizzas.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace DCasaPizzas.Logic
{
    class Pedidos
    { 
        public async Task<ObservableCollection<Pedido>> ListarPedidos()
        {
            ObservableCollection<Pedido> pedidos = new ObservableCollection<Pedido>();
            try
            {
                string sdsUrl = "Pedido/ListarPedidos?IdUsuario=" + App.IdUsuario;
                var response = await RequestWS.RequestGET(sdsUrl);
                response.EnsureSuccessStatusCode();
                var retorno = await response.Content.ReadAsStringAsync();

                pedidos = (ObservableCollection<Pedido>)JsonConvert.DeserializeObject(retorno, typeof(ObservableCollection<Pedido>));

                return pedidos;
            }
            catch
            {
                throw;
            }
        }
    }
}
