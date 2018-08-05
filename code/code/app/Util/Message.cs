using AppRomagnole.Models;
using Xamarin.Forms;

namespace AppRomagnole.Util
{
    public static class MessageToast
    {
        public static void ShortMessage(string message)
        {
            DependencyService.Get<IMessage>().ShortAlert(message);
        }

        public static void LongMessage(string message)
        {
            DependencyService.Get<IMessage>().LongAlert(message);
        }

        public static void Notificacao(string message)
        {
            DependencyService.Get<IMessage>().Notificacao(message);
        }
    }
}
