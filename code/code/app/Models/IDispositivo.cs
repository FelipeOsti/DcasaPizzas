using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppRomagnole.Models
{
    public interface IDispositivo
    {
        Task<ModelDispositivo> GetDispositivo();
        Task<bool> SolicitaPermissao();
    }
}
