using System;
using System.Collections.Generic;
using System.Text;

namespace AppRomagnole.Models
{
    public interface IMessage
    {
        void LongAlert(string message);
        void ShortAlert(string message);
        void Notificacao(string message);
    }
}
