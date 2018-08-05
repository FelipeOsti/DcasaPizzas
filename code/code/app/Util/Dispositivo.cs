using AppRomagnole.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppRomagnole.Util
{
    class Dispositivo
    {
        public async Task<ModelDispositivo> GetDispositivo ()
        {
            return await DependencyService.Get<IDispositivo>().GetDispositivo();
        }

        public async Task<bool> SolicitaPermissao()
        {
            return await DependencyService.Get<IDispositivo>().SolicitaPermissao();
        }
    }
}
