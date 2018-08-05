using AppRomagnole.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppRomagnole.Logic
{
    class ListaPlanilhas
    {
        public static async Task<List<PlanilhaFinanceira>> ListarAsync(string scdUsuar)
        {
            try
            {
                var client = new HttpClient { BaseAddress = new Uri(MainPage.clientReturnURI) };

                string authHeader = MainPage.auth.auth.CreateAuthorizationHeader();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, MainPage.clientReturnURI+"getPlanilhas?scdUsuar=" +scdUsuar);
                request.Headers.TryAddWithoutValidation("Authorization", authHeader);
                HttpResponseMessage response = await client.SendAsync(request);
                string responseString = await response.Content.ReadAsStringAsync();

                var response = await client.GetAsync("controladora/buscaContEstufa?nidControla=" + nidControla + "&ncdRede=" + ncdRede);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var lista = JsonConvert.DeserializeObject<List<PR_CONTESTUFA>>(json);

                return lista;
            }
            catch (HttpRequestException he)
            {
                await LogErroPost.gravarErro(he);
                throw;
            }
            catch (Exception ex)
            {
                await LogErroPost.gravarErro(ex);
                throw;
            }
        }
    }
}
