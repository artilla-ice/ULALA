using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using ULALA.Models;
using Windows.UI.Xaml.Controls;

namespace ULALA
{
    public partial class MainPage : Page, INotifyPropertyChanged
    {
        public MainPage()
        {
            this.InitializeComponent();
            OnLoadMenuItems();
        }

        private void OnLoadMenuItems()
        {
            var menuItems = new List<MenuItemModel>()
            { 
                new MenuItemModel("Assets/Icons/Cobrar.png", "Cobrar", Color.DarkGray.Name, null),
                new MenuItemModel("Assets/Icons/Estatus.png", "Estado de CashLogy", Color.CadetBlue.Name, null),
                new MenuItemModel("Assets/Icons/AddCambios.png", "Añadir cambios", Color.DarkRed.Name, null),
                new MenuItemModel("Assets/Icons/Cambio.png", "Dar Cambio", Color.MediumVioletRed.Name, null),
                new MenuItemModel("Assets/Icons/RetirarEfectivo.png", "Retirar Efectivo", Color.Green.Name, null),

                new MenuItemModel("Assets/Icons/Cobrar.png", "Retirar Stacker", Color.DarkSlateGray.Name, null),
                new MenuItemModel("Assets/Icons/Estatus.png", "Fondo de Caja", Color.YellowGreen.Name, null),
                new MenuItemModel("Assets/Icons/AddCambios.png", "Vaciado", Color.SkyBlue.Name, null),
                new MenuItemModel("Assets/Icons/Cambio.png", "Estadisticas ABS", Color.DarkViolet.Name, null),
                new MenuItemModel("Assets/Icons/Cobrar.png", "Estadisticas Red", Color.Blue.Name, null),
                new MenuItemModel("Assets/Icons/Estatus.png", "Mantenimiento", Color.Violet.Name, null),
                new MenuItemModel("Assets/Icons/AddCambios.png", "Logs", Color.Red.Name, null),};

            this.MenuItems = new ObservableCollection<MenuItemModel>(menuItems);
        }


        private ObservableCollection<MenuItemModel> m_menuItems;
        public ObservableCollection<MenuItemModel> MenuItems
        {
            get { return m_menuItems; }
            set { SetProperty(ref m_menuItems, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
