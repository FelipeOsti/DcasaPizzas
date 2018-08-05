using AppRomagnole.Models;
using AppRomagnole.Util;
using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppRomagnole.Logic
{
    class RequestWS
    {
        public static async Task<HttpResponseMessage> RequestGET(string sdsUrl)
        {
            try
            {
                ConexaoWeb conn = new ConexaoWeb();
                if (!await conn.IsConnected())
                {
                    MessageToast.LongMessage("Verifique sua conexão com a internet!");
                    return new HttpResponseMessage();
                }
            
                var client = new HttpClient(new NativeMessageHandler()) { BaseAddress = new Uri(MainPage.apiURI) };

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                string authHeader = MainPage.adfs.auth.CreateAuthorizationHeader();
                client.DefaultRequestHeaders.Add("Authorization", authHeader);
                var response = await client.GetAsync(sdsUrl);

                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (Exception ex)
            {
                //MessageToast.LongMessage(ex.Message);
                throw;
            }
        }
        
        public static async Task<HttpResponseMessage> RequestPOST(string sdsUrl, string json)
        {
            try
            {
                ConexaoWeb conn = new ConexaoWeb();
                if (!await conn.IsConnected())
                {
                    MessageToast.LongMessage("Verifique sua conexão com a internet!");
                    return new HttpResponseMessage();
                }

                var client = new HttpClient(new NativeMessageHandler()) { BaseAddress = new Uri(MainPage.apiURI) };

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                string authHeader = MainPage.adfs.auth.CreateAuthorizationHeader();
                client.DefaultRequestHeaders.Add("Authorization", authHeader);

                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                var response = await client.PostAsync(sdsUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();

                return response;
            }
            catch(Exception ex)
            {
                throw;// MessageToast.LongMessage(ex.Message);
                //return new HttpResponseMessage();
            }
        }
    }
}
