using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using ULALA.Models;
using ULALA.ViewModels;
using Windows.UI.Xaml.Controls;

namespace ULALA.Views
{
    public partial class ControlPanelView : Page
    {
        public ControlPanelView()
        {
            this.InitializeComponent();
        }

        private void controlPanelNV_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            var vm = this.DataContext as ControlPanelViewModel;
            if (vm != null)
            {
                vm.OnGoBack();
            }
        }
    }
}
