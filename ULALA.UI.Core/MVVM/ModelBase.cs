using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using ULALA.UI.Core.Contracts.MVVM;
using Xamarin.Forms;
using ULALA.UI.Core.Contracts.Navigation;

namespace ULALA.UI.Core.MVVM
{
    public class ModelBase : INotifyPropertyChanged
    {
        protected bool SetProperty<T>(ref T backingStore, T value,[CallerMemberName] string propertyName = "",Action onChanged = null)
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
