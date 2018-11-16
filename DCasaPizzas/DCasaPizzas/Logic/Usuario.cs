using DCasaPizzas.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DCasaPizzas.Logic
{
    class Usuario
    {
        public async Task<string> CriarUsuario(UsuarioModel user, bool bboAlterar)
        {
            try
            {
                if (user.DS_SENHA == "" || user.DS_SENHA == null)
                    user.BO_FACEBOOK = "T";                    
                else
                    user.DS_SENHA = CriptografaSHA256(user.DS_SENHA);

                string sdsUrl = "Usuario/VerificaUsuarioExiste?sdsEmail=" + user.DS_EMAIL;
                var response = await RequestWS.RequestGET(sdsUrl);
                var retorno = await response.Content.ReadAsStringAsync();
                retorno = retorno.Replace("\"", "");
                if (retorno == "T" && !bboAlterar)
                {            
                    return retorno;
                }

                var json = JsonConvert.SerializeObject(user);

                sdsUrl = "Usuario/NovoUsuario";
                response = await RequestWS.RequestPOST(sdsUrl, json);

                response.EnsureSuccessStatusCode();

                return "";
            }
            catch
            {
                throw;
            }
        }

        internal async Task<UsuarioModel> GetDadosUsuario(string sdsEmail)
        {
            string sdsUrl = "Usuario/GetDadosUsuario?sdsEmail=" + sdsEmail;
            var response = await RequestWS.RequestGET(sdsUrl);

            string retorno = await response.Content.ReadAsStringAsync();

            UsuarioModel user = new UsuarioModel();
            return (UsuarioModel)JsonConvert.DeserializeObject(retorno, typeof(UsuarioModel));
        }

        internal async Task NovaSenha(string email)
        {
            string sdsUrl = "Usuario/CriarSenha?sdsEmail=" + email;
            var response = await RequestWS.RequestGET(sdsUrl);

            string retorno = await response.Content.ReadAsStringAsync();
            if (retorno.Replace("\"", "") != "") throw new Exception("Falha ao criar nova senha! - " + retorno.Replace("\"", ""));
        }

        internal async Task<bool> VerificaEmailExiste(string email)
        {
            string sdsUrl = "Usuario/GetNome?sdsEmail=" + email;
            var response = await RequestWS.RequestGET(sdsUrl);

            string sdsNome = await response.Content.ReadAsStringAsync();

            if (sdsNome.Replace("\"", "") != "" && sdsNome.Replace("\"", "") != null) return true;
            return false;
        }

        public string CriptografaSHA256(string senha)
        {
            try
            {

                SHA256Managed crypt = new SHA256Managed();
                StringBuilder hash = new StringBuilder();
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(senha), 0, Encoding.UTF8.GetByteCount(senha));
                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                }
                var retorno = hash.ToString();
                return retorno.TrimStart('"').TrimEnd('"');

            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> VerificaUsuarioSenha(UsuarioModel user)
        {
            try
            {
                user.DS_SENHA = CriptografaSHA256(user.DS_SENHA);

                string sdsUrl = "Usuario/VerificaUsuarioSenha?sdsEmail=" + user.DS_EMAIL+"&sdsSenha="+user.DS_SENHA;
                var response = await RequestWS.RequestGET(sdsUrl);
                var retorno = await response.Content.ReadAsStringAsync();
                retorno = retorno.Replace("\"", "");
                if (retorno == "T")
                    return true;
            
                return false;
            }
            catch
            {
                throw;
            }
        }

        internal async Task<bool> SenhaProvisoria()
        {
            string sdsUrl = "Usuario/SenhaProvisoria?sdsEmail=" + App.sdsEmail;
            var response = await RequestWS.RequestGET(sdsUrl);
            var retorno = await response.Content.ReadAsStringAsync();
            retorno = retorno.Replace("\"", "");
            if (retorno == "T")
                return true;

            return false;
        }

        internal async Task<string> GetNome()
        {
            try
            {

                string sdsUrl = "Usuario/GetNome?sdsEmail=" + App.sdsEmail;
                var response = await RequestWS.RequestGET(sdsUrl);

                string sdsNome = await response.Content.ReadAsStringAsync();

                return sdsNome.Replace("\"","");
            }
            catch
            {
                return "";
            }
        }

        internal async Task<long> GetIDUsuario(string email)
        {
            try
            {

                string sdsUrl = "Usuario/GetIDUsuario?sdsEmail="+email;
                var response = await RequestWS.RequestGET(sdsUrl);

                string sdsId = await response.Content.ReadAsStringAsync();
                long id = (long)double.Parse(sdsId.Replace(".0",""));

                return id;
            }
            catch
            {
                return 0;
            }
        }
    }
}
