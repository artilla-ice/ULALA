using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ULALA.Models
{
    public class MenuItemModel : INotifyPropertyChanged
    {
        public MenuItemModel(string icon, string name, string color, string viewName)
        {
            this.Icon = icon;
            this.Name = name;
            this.Color = color;
            this.ViewName = viewName;
        }


        private string m_icon;
        public string Icon
        {
            get { return m_icon; }
            set { SetProperty(ref m_icon, value); }
        }



        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { SetProperty(ref m_name, value); }
        }

        private string m_color;
        public string Color
        {
            get { return m_color; }
            set { SetProperty(ref m_color, value); }
        }

        private string m_viewName;
        public string ViewName
        {
            get { return m_viewName; }
            set { SetProperty(ref m_viewName, value); }
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
