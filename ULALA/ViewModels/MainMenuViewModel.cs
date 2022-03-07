using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Models;
using ULALA.UI.Core.Contracts.Navigation;
using ULALA.UI.Core.MVVM;
using ULALA.Views;
using Xamarin.Forms;
using Color = System.Drawing.Color;

namespace ULALA.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        public Command ConfigurationCommand { get; set; }

        public MainMenuViewModel()
        {
            this.ConfigurationCommand = new Command(OnLoadControlPanel);
        }

        protected override void OnActivated()
        {
            this.OHLALALogo = "../Assets/Icons/OHLALABoulangerieBistrotLogo.svg";
            OnLoadMenuItems();
        }

        private void OnLoadMenuItems()
        {
            var menuItems = new List<MenuItemModel>()
            {
                new MenuItemModel("../Assets/Icons/Cobrar.png", "Cobrar", Color.BlueViolet.Name, ViewNames.NewCharge),
                //new MenuItemModel("../Assets/Icons/Estatus.png", "Estado de CashLogy", Color.CadetBlue.Name, null),
                new MenuItemModel("../Assets/Icons/AddCambios.png", "Añadir cambios", Color.DarkRed.Name, ViewNames.AddExchange),
                //new MenuItemModel("../Assets/Icons/Cambio.png", "Dar Cambio", Color.MediumVioletRed.Name, null),
               // new MenuItemModel("../Assets/Icons/RetirarEfectivo.png", "Retirar Efectivo", Color.Green.Name, ViewNames.WithdrawCash),
                //TODO: APARTIR DE AQUI LOS ICONOS ESTAN DUPLICADOS, REEMPLAZAR                   
                new MenuItemModel("../Assets/Icons/Cobrar.png", "Retirar Stacker", Color.DarkSlateGray.Name, ViewNames.WithdrawStacker),
                new MenuItemModel("../Assets/Icons/Estatus.png", "Fondos de Caja", Color.YellowGreen.Name, ViewNames.CashFunds),
               // new MenuItemModel("../Assets/Icons/AddCambios.png", "Vaciado", Color.SkyBlue.Name, null ),
                //new MenuItemModel("../Assets/Icons/Cambio.png", "Estadisticas ABS", Color.DarkViolet.Name, null),
                //new MenuItemModel("../Assets/Icons/Cobrar.png", "Estadisticas Red", Color.Blue.Name, null),
                //new MenuItemModel("../Assets/Icons/Estatus.png", "Mantenimiento", Color.Violet.Name, null),
                new MenuItemModel("../Assets/Icons/AddCambios.png", "Logs", Color.Red.Name, ViewNames.Logs),};
            
            this.MenuItems = new ObservableCollection<MenuItemModel>(menuItems);
        }

        private void OnNavigateToSelectedMenuItem()
        {
            if(m_selectedMenuItem != null)
            {
                this.NavigationManager.NavigateTo(m_selectedMenuItem.ViewName);
            }
        }

        private void OnLoadControlPanel()
        {
            this.NavigationManager.NavigateTo(ViewNames.ControlPanel);
        }

        private ObservableCollection<MenuItemModel> m_menuItems;
        public ObservableCollection<MenuItemModel> MenuItems
        {
            get { return m_menuItems; }
            set { SetProperty(ref m_menuItems, value); }
        }

        private string m_OHLALALogo;
        public string OHLALALogo
        {
            get { return m_OHLALALogo; }
            set { SetProperty(ref m_OHLALALogo, value); }
        }

        private MenuItemModel m_selectedMenuItem;
        public MenuItemModel SelectedMenuItem
        {
            get { return m_selectedMenuItem; }
            set 
            { 
                if(value != m_selectedMenuItem)
                {
                    SetProperty(ref m_selectedMenuItem, value);
                    OnNavigateToSelectedMenuItem();
                }
            }
        }
    }
}
