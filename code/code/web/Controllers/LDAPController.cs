using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.DirectoryServices;
using System.Text;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Web;
using WebAppRoma.Models;
using System.Drawing.Imaging;

namespace WebAppRoma.Controllers
{
    [Authorize]
    [RoutePrefix("API/ldap")]
    public class LDAPController : ApiController
    {
        private String _path = "LDAP://romagnole.local";
        private String domain = "romagnole";
        private String username = "romanet";
        private String pwd = "roma12@";

        [HttpGet]
        [Route("GetUsuario")]
        public List<string> GetUsuario(String sdsEmail)
        {
            String domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);

            try
            {//Bind to the native AdsObject to force authentication.
                Object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);

                string sdsNome = "";

                search.Filter = "(mail=" + sdsEmail + ")";
                search.PropertiesToLoad.Add("displayname");
                SearchResultCollection results = search.FindAll();
                if (results != null)
                {
                    foreach (SearchResult sresult in results)
                    {
                        foreach (DictionaryEntry property in sresult.Properties)
                        {
                            if (property.Key.ToString() == "displayname")
                            {
                                foreach (var val in (property.Value as ResultPropertyValueCollection))
                                {
                                    sdsNome = val.ToString();
                                }
                            }
                        }
                    }
                }
                else { return null; }

                List<string> sdsRetorno = new List<string>();

                sdsRetorno.Add(sdsNome);
                return sdsRetorno;
            }
            catch (Exception ex)
            {
                throw;
            }            
        }    

        [Route("GetUsuarioAD")]
        [HttpGet]
        public UsuarioAD GetUsuarioAD(string sdsEmail)
        {
            String domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);

            using (DirectorySearcher dsSearcher = new DirectorySearcher())
            {
                dsSearcher.Filter = string.Format("(&(mail={0}))", sdsEmail);
                SearchResult result = dsSearcher.FindOne();

                using (DirectoryEntry user = new DirectoryEntry(result.Path))
                {
                    var sdsNome = user.Properties["cn"].Value as string;
                    var sdsCargo = user.Properties["title"].Value as string;
                    var sdsCcusto = user.Properties["description"].Value as string;
                    return new UsuarioAD() {
                        email = sdsEmail,
                        cargo = sdsCargo,
                        ccusto = sdsCcusto,
                        nome = sdsNome
                    };
                }
            }
        }

        public List<String> GetGroups(String _filterAttribute)
        {
            DirectorySearcher search = new DirectorySearcher(_path);
            search.Filter = "(cn=" + _filterAttribute + ")";
            search.PropertiesToLoad.Add("memberOf");
            List<string> groupNames = new List<string>();

            try
            {
                SearchResult result = search.FindOne();

                int propertyCount = result.Properties["memberOf"].Count;

                String dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    dn = (String)result.Properties["memberOf"][propertyCounter];

                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return null;
                    }

                    groupNames.Add(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                }

                return groupNames;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetUserPicture")]
        public string GetUserPicture(string sdsEmail)
        {
            try
            {
                string sdsFileDir = System.Configuration.ConfigurationManager.AppSettings["ida:Audience"] + @"site/img/profile";
                string sdsUrlName = sdsFileDir + @"/" + sdsEmail.Replace(".", "") + ".bmp";
                string sdsFileName = HttpContext.Current.Request.MapPath("~") + @"site\img\profile\" + sdsEmail.Replace(".", "") + ".bmp";

                if (File.Exists(sdsFileName))
                {
                    return sdsUrlName;
                }
                else
                {
                    String domainAndUsername = domain + @"\" + username;
                    DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);

                    using (DirectorySearcher dsSearcher = new DirectorySearcher())
                    {
                        dsSearcher.Filter = string.Format("(&(mail={0}))", sdsEmail);
                        SearchResult result = dsSearcher.FindOne();

                        using (DirectoryEntry user = new DirectoryEntry(result.Path))
                        {
                            byte[] data = user.Properties["thumbnailPhoto"].Value as byte[];
                            if (data != null)
                            {
                                using (MemoryStream s = new MemoryStream(data))
                                {
                                    MemoryStream ms = new MemoryStream(data, 0, data.Length);
                                    ms.Write(data, 0, data.Length);
                                    Image image = Image.FromStream(ms, true);
                                    image.Save(sdsFileName, ImageFormat.Bmp);

                                    //var bmp = Bitmap.FromStream(s);
                                    //bmp.Save(sdsFileName,ImageFormat.Bmp);

                                    return sdsUrlName;
                                }
                            }

                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
    
}
