using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Zeus;
using ULALA.Models;
using ULALA.UI.Core.Contracts.Navigation;
using ULALA.UI.Core.MVVM;
using Unity;

namespace ULALA.ViewModels
{
    public class ZeusConnectionSettingsViewModel : ViewModelBase
    {
        [Dependency]
        public IZeusManager ZeusManager { get; set; }

        public Xamarin.Forms.Command StartZeusConnectionCommand { get; set; }
        public Xamarin.Forms.Command StopZeusConnectionCommand { get; set; }

        public ZeusConnectionSettingsViewModel()
        {
            this.StartZeusConnectionCommand = new Xamarin.Forms.Command(OnStartConnection);
            this.StopZeusConnectionCommand = new Xamarin.Forms.Command(OnCloseConnection);
        }

        protected override void OnActivated()
        {
            this.IsConnected = this.ZeusManager.IsConnected;
        }

        private void OnStartConnection()
        {
            this.ZeusManager.OnStartListening();

            this.IsConnected = this.ZeusManager.IsConnected;
        }

        private void OnCloseConnection()
        {
            this.ZeusManager.OnCloseConnection();

            this.IsConnected = this.ZeusManager.IsConnected;

        }

        private bool m_isConnected = false;
        public bool IsConnected
        {
            get { return m_isConnected; }
            set { SetProperty(ref m_isConnected, value); }
        }

    }
}
