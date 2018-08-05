using DCasaPizzas.Models;
using DCasaPizzas.Util;
using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DCasaPizzas.Logic
{
    class RequestWS
    {
        public static async Task<HttpResponseMessage> RequestGET(string sdsUrl)
        {
            try
            {

                var client = new HttpClient(new NativeMessageHandler()) { BaseAddress = new Uri(MainPage.apiURI) };

                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = await client.GetAsync(sdsUrl);

                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public static async Task<HttpResponseMessage> RequestPOST(string sdsUrl, string json)
        {
            try
            {

                var client = new HttpClient(new NativeMessageHandler()) { BaseAddress = new Uri(MainPage.apiURI) };

                client.DefaultRequestHeaders.Add("Accept", "application/json");

                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                var response = await client.PostAsync(sdsUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();

                return response;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
