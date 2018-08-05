using DCasaPizzas.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DCasaPizzas.Util
{
    class ConexaoWeb
    {
        public async Task<bool> IsConnected()
        {
            return await DependencyService.Get<IConexaoWeb>().IsConnected();
        }
    }
}
