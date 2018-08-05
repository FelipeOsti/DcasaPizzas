using AppRomagnole.Models;
using AppRomagnole.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AppRomagnole.Logic
{
    class AuthDispositivoController
    {
        private string email;

        public AuthDispositivoController(string _email)
        {
            email = _email;
        }

        public async Task<bool> ValidaCodigoAprovacaoAsync(string codigo) {
            Dispositivo disp = new Dispositivo();
            ModelDispositivo dispInfo = await disp.GetDispositivo();
            dispInfo.email = email;

            codigo = codigo.ToUpper();

            try
            {
                string json = JsonConvert.SerializeObject(dispInfo);
                string sdsUrl = "dispositivo/ValidaCodigoAprovacao?codigo=" + codigo+ "&emailRequis=" + MainPage.sdsEmail;
                var response = await RequestWS.RequestPOST(sdsUrl, json);                

                string sboOk = await response.Content.ReadAsStringAsync();
                sboOk = sboOk.Replace("\"", "");

                return sboOk == "T";
            }
            catch
            {
                throw;
            }
        }

        internal async Task<bool> AprovaCodigoAPP(string codigo)
        {
            Dispositivo disp = new Dispositivo();
            ModelDispositivo dispInfo = await disp.GetDispositivo();
            dispInfo.email = email;

            try
            {
                string json = JsonConvert.SerializeObject(dispInfo);
                var sdsUrl = "dispositivo/AprovaCodigoAPP?codigo=" + codigo + "&emailRequis=" + MainPage.sdsEmail;
                var response = await RequestWS.RequestPOST(sdsUrl, json);

                string sboAuth = await response.Content.ReadAsStringAsync();
                sboAuth = sboAuth.Replace("\"", "");
                if (sboAuth == "T")
                    MessageToast.Notificacao("Dispositivo liberado com sucesso! Seja bem vindo!");

                return sboAuth == "T";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DispositivoAutorizadoAsync()
        {
            try
            {
                Dispositivo disp = new Dispositivo();
                ModelDispositivo dispInfo = await disp.GetDispositivo();
                dispInfo.email = email;
           
                if (dispInfo.IMEI == "")
                {
                    MessageToast.LongMessage("Permissões de acesso ao aparelho negadas pelo usuário!");
                    throw new Exception("Acesso ao aparelho negada pelo usuário.");
                }

                string json = JsonConvert.SerializeObject(dispInfo);                
                string sdsUrl = "dispositivo/DispositivoAutorizado?emailRequis=" + MainPage.sdsEmail;
                var response = await RequestWS.RequestPOST(sdsUrl, json);

                string sboAuth = await response.Content.ReadAsStringAsync();
                sboAuth = sboAuth.Replace("\"", "");
                if(sboAuth == "A")
                    MessageToast.Notificacao("Dispositivo aguardando liberação. Verifique seu e-mail!");

                return sboAuth;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
