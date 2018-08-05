using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace AppRomagnole.Logic
{
    class PINController
    {
        public async Task<string> CriptografaAsync(string pin)
        {
            try
            {
                var sdsUrl = "pin/CriptografaSHA256?pin=" + pin;
                var response = await RequestWS.RequestGET(sdsUrl);
                var pinCripto = await response.Content.ReadAsStringAsync();
                return pinCripto.TrimStart('"').TrimEnd('"');
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> GetPINEmail (string sdsEmail)
        {
            try
            {
                var sdsUrl = "pin/GetPINEmail?email=" + sdsEmail;
                var response = await RequestWS.RequestGET(sdsUrl);
                var pinCripto = await response.Content.ReadAsStringAsync();
                return pinCripto;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> PossuiPIN (string sdsEmail)
        {
            try
            {
                string pin = await GetPINEmail(sdsEmail);
                if (pin == "null") return false;
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> VerificaPIN(string pin, string sdsEmail)
        {
            try
            {
                var sdsUrl = "pin/VerificaPIN?pin=" + pin + "&email=" + sdsEmail;
                var response = await RequestWS.RequestGET(sdsUrl);

                var bboOK = await response.Content.ReadAsStringAsync();
                if(bboOK == "true") return true;
                return false;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> SalvarPin(string sdsPin, string sdsEmail)
        {
            try
            {
                PIN conteudo = new PIN
                {
                    DS_PIN = sdsPin,
                    DS_EMAIL = sdsEmail
                };
                string json = JsonConvert.SerializeObject(conteudo);

                var sdsUrl = "pin/SalvarPin";
                var response = await RequestWS.RequestPOST(sdsUrl, json);
                var bboOk = await response.Content.ReadAsStringAsync();
                if(bboOk == "true") return true;
                return false;
            }
            catch
            {
                throw;
            }
        }
    }

    public class PIN
    {
        public string DS_PIN { get; set; }
        public string DS_EMAIL { get; set; }
    }

}
