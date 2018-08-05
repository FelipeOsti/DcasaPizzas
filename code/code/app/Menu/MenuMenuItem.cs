using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppRomagnole.Menu
{
    public class ItemMenu
    {
        public ItemMenu()
        {
            TargetType = typeof(MenuDetail);
        }
        public string Title { get; set; }
        public Color TitleColor { get; set; }
        public FontAttributes TitleWeight { get; set; }
        public string IconSource { get; set; }
        public Type TargetType { get; set; }
        public Boolean bboForm { get; set; }

    }
}