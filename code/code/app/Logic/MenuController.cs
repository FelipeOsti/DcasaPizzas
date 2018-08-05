using AppRomagnole.Menu;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using AppRomagnole.Models;

namespace AppRomagnole.Logic
{
    class MenuController
    {
        public async Task<ObservableCollection<GroupMenu>> GetMenu()
        {
            try
            {
                ObservableCollection<GroupMenu> lstGrpMenu = new ObservableCollection<GroupMenu>();

                var sdsUrl = "menu/GetMenu?sdsEmail=" + MainPage.sdsEmail;
                var response = await RequestWS.RequestGET(sdsUrl);

                var retornoJson = await response.Content.ReadAsStringAsync();
                var lstMenu = JsonConvert.DeserializeObject<List<GroupItem>>(retornoJson);

                foreach (GroupItem grp in lstMenu)
                {
                    var grupo = new GroupMenu() { GroupName = grp.GroupName };
                    foreach (ItemMenu item in grp.ItensMenu)
                    {
                        var itemMenu = new ItemMenu()
                        {
                            Title = item.Title,
                            TitleColor = Color.Gray,
                            IconSource = item.IconSource,
                            TargetTypeType = Type.GetType(item.TargetType),
                            bboForm = item.bboForm,
                            isSelected = false
                        };
                        if (item.TargetType == "AppRomagnole.Menu.HomePage") itemMenu.isSelected = true;

                        grupo.Add(itemMenu);

                        if (item.lstGraficos != null)
                        {
                            itemMenu.lstGraficos = new List<GraficoURL>();
                            foreach (GraficoURL graf in item.lstGraficos)
                            {
                                itemMenu.lstGraficos.Add(graf);
                            }
                        }
                    }
                    lstGrpMenu.Add(grupo);
                }

                return lstGrpMenu;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> FavoritarGrafico(double IdMenuApp, bool bboFav)
        {
            try
            {
                string sdsUrl = MainPage.apiURI + "menu/FavGrafico?IdMenuAPP=" + IdMenuApp + "&bboFav=" + bboFav + "&sdsEmail=" + MainPage.sdsEmail;
                var retorno = await RequestWS.RequestGET(sdsUrl);

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> VerificaGraficoFavorito (double IdMenuApp)
        {
            try
            {
                string sdsUrl = MainPage.apiURI + "menu/VerificaFavorito?IdMenuApp=" + IdMenuApp + "&sdsEmail=" + MainPage.sdsEmail;
                var retorno = await RequestWS.RequestGET(sdsUrl);
                var retornoString = await retorno.Content.ReadAsStringAsync();
                return retornoString.ToLower() == "true";
            }
            catch { return false; }
        }

        public async Task<List<Models.Grafico>> GetInicio(){
            try
            {
                List<Models.Grafico> lstInicio = new List<Models.Grafico>();
                string sdsUrl = MainPage.apiURI+"menu/GetInicio?sdsEmail="+MainPage.sdsEmail;
                var retorno = await RequestWS.RequestGET(sdsUrl);

                var retornoJson = await retorno.Content.ReadAsStringAsync();
                lstInicio = JsonConvert.DeserializeObject<List<Models.Grafico>>(retornoJson);
                return lstInicio;
            }
            catch
            {
                return null;
            }
        }
    }
}
