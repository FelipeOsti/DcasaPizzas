using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCasaPizzas.Models
{
    public interface IConexaoWeb
    {
        Task<bool> IsConnected();
    }
}
