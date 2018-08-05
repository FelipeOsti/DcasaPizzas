using DCasaPizzas.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace DCasaPizzas.Logic
{
    class Fidelidade
    {
        public async Task<double> GetPontos(string sdsEmail)
        {
            try
            {
                var retornoWS = await RequestWS.RequestGET("Fidelidade/GetPontos?sdsParam="+ sdsEmail);

                string retorno = await retornoWS.Content.ReadAsStringAsync();

                return double.Parse(retorno.Replace(".",","));
            }
            catch
            {
                return 0;
            }
        }

        public async Task<string> ValidarToken(string sdsToken)
        {
            try
            {
                var retornoWS = await RequestWS.RequestGET("Fidelidade/ValidarToken?NrToken=" + sdsToken + "&IdUsuario=" + App.IdUsuario);

                string retorno = await retornoWS.Content.ReadAsStringAsync();

                return retorno.Replace("\"","");
            }
            catch
            {
                return "";
            }
        }

        internal async Task<ObservableCollection<Token>> ListaPontosExpirando()
        {
            try
            {
                var retornoWS = await RequestWS.RequestGET("Fidelidade/ListarPontosExpirando?idUsuario=" + App.IdUsuario);

                string retorno = await retornoWS.Content.ReadAsStringAsync();

                var lstRetorno = (ObservableCollection<Models.Token>)JsonConvert.DeserializeObject(retorno, typeof(ObservableCollection<Models.Token>));

                return lstRetorno;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ObservableCollection<Models.Token>> ListarPontos()
        {
            try
            {
                var retornoWS = await RequestWS.RequestGET("Fidelidade/ListarPontos?idUsuario=" + App.IdUsuario);

                string retorno = await retornoWS.Content.ReadAsStringAsync();

                var lstRetorno = (ObservableCollection<Models.Token>)JsonConvert.DeserializeObject(retorno, typeof(ObservableCollection<Models.Token>));

                return lstRetorno;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Models.Trocas>> GetTrocas()
        {
            try
            {
                var retornoWS = await RequestWS.RequestGET("Fidelidade/GetTrocas?idUsuario=" + App.IdUsuario);

                string retorno = await retornoWS.Content.ReadAsStringAsync();

                var lstRetorno = (List<Models.Trocas>)JsonConvert.DeserializeObject(retorno, typeof(List<Models.Trocas>));

                return lstRetorno;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> SalvarTroca(List<Models.Produto> produtos)
        {
            try
            {
                long nnrPontos = 0;
                foreach (Models.Produto prod in produtos)
                    nnrPontos += (long)prod.NR_PONTOS;

                string sdsJson = JsonConvert.SerializeObject(produtos);

                var retornoWS = await RequestWS.RequestPOST("Fidelidade/GravarTroca?idUsuario=" + App.IdUsuario + "&nrPontos=" + nnrPontos, sdsJson);

                string retorno = await retornoWS.Content.ReadAsStringAsync();

                var lstRetorno = retorno.Replace("\"", "");

                return lstRetorno == "T";
            }
            catch
            {
                return false;
            }
        }
    }
}
