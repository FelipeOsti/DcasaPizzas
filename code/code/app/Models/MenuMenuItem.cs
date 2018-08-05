using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppRomagnole.Menu
{
    public class GraficoURL
    {
        public double ID_MENUAPP { get; set; }
        public string DS_URL { get; set; }
        public string DS_TITULO { get; set; }
        public int FL_TIPOGRAFICO { get; set; }
        public List<GraficoURL> lstGraficos { get; set; }
    }

    public class ItemMenu
    {
        public double nidMenu { get; set; }
        public string Title { get; set; }
        public Color TitleColor { get; set; }
        public string IconSource { get; set; }
        public string TargetType { get; set; }
        public Type TargetTypeType { get; set; }
        public Boolean bboForm { get; set; }
        public Boolean isSelected { get; set; }
        public Boolean isNotSelected { get { return !isSelected; } }
        public List<GraficoURL> lstGraficos { get; set; }
    }

    public class GroupMenu : ObservableCollection<ItemMenu>
    {
        public string GroupName { get; set; }
    }

    public class GroupItem
    {
        public string GroupName { get; set; }
        public List<ItemMenu> ItensMenu { get; set; }
    }    

}