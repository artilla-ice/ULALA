using System;
using System.Collections.Generic;
using System.Text;

namespace ULALA.UI.Core.Contracts.MVVM
{
    public interface IViewModelBase
    {
        void HandleActivated();
        void HandleDeactivated();
    }
}
