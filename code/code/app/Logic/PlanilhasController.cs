using AppRomagnole.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppRomagnole.Logic
{
    class PlanilhasController
    {
        public async Task<List<PlanilhaFinanceira>> ListarAsync()
        {
            try
            {
                var sdsUrl = "planfin/getPlanilhas?sdsEmail=" + MainPage.sdsEmail;
                var response = await RequestWS.RequestGET(sdsUrl);
                var json = await response.Content.ReadAsStringAsync();

                var lista = JsonConvert.DeserializeObject<List<PlanilhaFinanceira>>(json);

                return lista;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Parametro sflLiberReprova dever ser A ou R qualquer coisa diferente será entendido como A
        /// </summary>
        /// <param name="planilha"></param>
        /// <param name="sflLiberReprova"></param>
        /// <returns></returns>
        public async Task<string> LiberaReprovaPlanilha(PlanilhaFinanceira planilha, string sflLiberReprova)
        {
            //sflLiberReprova = A ou R
            try
            {
                planilha.DS_EMAIL = MainPage.sdsEmail;

                string jsonPlanilha = JsonConvert.SerializeObject(planilha);               
                var sdsUrl = "planfin/LiberaReprovaPlanilha?sflLiberReprova=" + sflLiberReprova + "&sdsEmail=" + MainPage.sdsEmail;
                var response = await RequestWS.RequestPOST(sdsUrl, jsonPlanilha);
                var retorno = await response.Content.ReadAsStringAsync();
                return retorno;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
