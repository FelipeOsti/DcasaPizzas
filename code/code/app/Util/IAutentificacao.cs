using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AppRomagnole.Util
{
    public interface IAutentificacao
    {
        Task<ModelAutenticacao> AutenticarAsync(string authority, string resource, string clientId, string returnUri);
        bool LogoutAsync(string authority);
    }

    public class ModelAutenticacao
    {
        public AuthenticationResult auth{ get; set; }
        public bool SegundaAutenticacao { get; set; }
    }
}
