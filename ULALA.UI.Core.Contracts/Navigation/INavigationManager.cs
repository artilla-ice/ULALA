using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ULALA.UI.Core.Contracts.Navigation
{
    public enum MainViewType
    {
        Undefined,
        LoginView,
        MainView
    }

    public interface INavigationManager
    {
        void Configure(string page, Type type);
        bool GoBack();
        void NavigateTo(string page);
        Task NavigateTo(MainViewType viewType);
        Task NavigateTo(string viewName, IDictionary<string, object> parameters);
        void RegisterView(string viewName, Type viewType);
        string CurrentPage { get; }
    }
}
