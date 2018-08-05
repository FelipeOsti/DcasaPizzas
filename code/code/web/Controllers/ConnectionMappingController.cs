using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebAppRoma.Hubs;
using WebAppRoma.Models;

namespace WebAppRoma.Controllers
{
    [Authorize]
    [RoutePrefix("api/connexao")]
    public class ConnectionMappingController : ApiController
    {
        [HttpGet]
        [Route("GetConexoes")]
        public Dictionary<string, HashSet<string>> GetConexoes()
        {
            var lstConn = ConnectionMapping<string>._connections;
            return lstConn;
        }

        [HttpPost]
        [Route("SendMessage")]
        public async Task<string> SendMessage(string userId, string message)
        {
            try
            {
                var proxy = ClientHUB.GetProxy();
                await proxy.Invoke("Send", new ModelMsgNotify() { sID = userId, sdsMsg = message });
                return "OK" + " - " + ClientHUB.getState();
            }
            catch (Exception ex)
            {
                return "ERRO: " + ex.Source + " - " + ex.Message+" - "+ ClientHUB.getState();
                //throw;
            }
        }

        private void OnMessage(string obj)
        {
            //
        }
    }
}
