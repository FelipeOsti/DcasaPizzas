using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace DCasaPizzas.Logic
{
    class Produto
    {
        public async Task<ObservableCollection<Models.Produto>> getProdutos()
        {
            try
            {
                var response = await RequestWS.RequestGET("Produto/GetProdutos");
                response.EnsureSuccessStatusCode();
                var retorno = await response.Content.ReadAsStringAsync();
                var lstProdutos = (ObservableCollection<Models.Produto>)JsonConvert.DeserializeObject(retorno,typeof(ObservableCollection<Models.Produto>));

                return lstProdutos;
            }
            catch
            {
                throw;
            }
        }
    }
}
