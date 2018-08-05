using System;
using System.Collections.Generic;
using System.Text;

namespace DCasaPizzas.Models
{
    public interface IMessage
    {
        void LongAlert(string message);
        void ShortAlert(string message);
        void Notificacao(string message);
    }
}
