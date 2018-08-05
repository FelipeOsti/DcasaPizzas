using AppRomagnole.Models;
using AppRomagnole.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppRomagnole.Logic
{
    class ldapController
    {
        public async Task<ImageSource> GetUserPicture(string sdsEmail)
        {
            try
            {
                var sdsUrl = "ldap/GetUserPicture?sdsEmail=" + sdsEmail;
                var response = await RequestWS.RequestGET(sdsUrl);
                var urlImg = await response.Content.ReadAsStringAsync();
                if (urlImg != "null")
                {
                    var imgS = ImageSource.FromUri(new Uri(urlImg.Replace("\"","")));
                    return imgS;
                }
                return null;
            }
            catch(Exception ex)
            {
                MessageToast.ShortMessage(ex.Message);
                return null;
            }
        }
        
        public async Task<UsuarioAD> GetUsuarioAD(string sdsEmail)
        {
            try
            {
                var sdsUrl = "ldap/GetUsuarioAD?sdsEmail=" + sdsEmail;
                var response = await RequestWS.RequestGET(sdsUrl);

                var json = await response.Content.ReadAsStringAsync();

                UsuarioAD usuar = JsonConvert.DeserializeObject<UsuarioAD>(json);

                return usuar;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
