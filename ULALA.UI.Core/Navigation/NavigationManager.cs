using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ULALA.Infrastructure.PubSub;
using ULALA.UI.Core.Contracts.Events;
using ULALA.UI.Core.Contracts.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ULALA.UI.Core.Navigation
{
    public class NavigationManager : INavigationManager
    {
        public NavigationManager()
        {
        }

        [Unity.Dependency]
        public IEventAggregator EventAggregator { get; set; }

        public void Configure(string page, Type type)
        {
            lock (m_viewsRegistry)
            {
                if (m_viewsRegistry.ContainsKey(page))
                    throw new ArgumentException("The specified page is already registered.");

                if (m_viewsRegistry.Values.Any(v => v == type))
                    throw new ArgumentException("The specified view has already been registered under another name.");

                m_viewsRegistry.Add(page, type);
            }
        }

        public string CurrentPage
        {
            get
            {
                var frame = AppFrame;
                if (frame.BackStackDepth == 0)
                    return RootPage;

                if (frame.Content == null)
                    return UnknownPage;

                var type = frame.Content.GetType();

                lock (m_viewsRegistry)
                {
                    if (m_viewsRegistry.Values.All(v => v != type))
                        return UnknownPage;

                    var item = m_viewsRegistry.Single(i => i.Value == type);

                    return item.Key;
                }
            }
        }

        public void GoBack()
        {
            System.Diagnostics.Debug.Assert(AppFrame != null);
            if (AppFrame.CanGoBack)
                AppFrame.GoBack();
        }

        public void NavigateTo(string page)
        {
            NavigateTo(page, null);
        }

        public Task NavigateTo(MainViewType viewType)
        {
            this.EventAggregator.GetEvent<ExecuteUINavigationEvent>().Publish(new ExecuteUINavigationEventArgs
            {
                ViewType = viewType
            });

            return Task.CompletedTask;
        }

        public Task NavigateTo(string viewName, IDictionary<string, object> parameters)
        {
            if(viewName != null || string.IsNullOrEmpty(viewName))
            {
                if (m_viewsRegistry.TryGetValue(viewName, out Type viewType))
                {
                    Page view;
                    if (parameters == null)
                        view = (Page)Activator.CreateInstance(viewType);
                    else
                        view = (Page)Activator.CreateInstance(viewType, parameters);

                    AppFrame.Navigate(m_viewsRegistry[viewName], parameters);
                }
                else
                    throw new ArgumentException($"NavigationManager, view not registered: {viewName}");
            }

            return Task.CompletedTask;
        }

        public void RegisterView(string viewName, Type viewType)
        {
            var currentType = viewType.GetTypeInfo().BaseType;
            if (currentType != typeof(Page))
                throw new ArgumentException($"Navigation Manager class: {viewType.Name} must inherit from Page to register it");

            m_viewsRegistry.Add(viewName, viewType);
        }

        private const string RootPage = "(Root)";
        private const string UnknownPage = "(Unknown)";
        private static Frame AppFrame => ((Window.Current.Content as Frame)?.Content as Page)?.Frame;

        private Dictionary<string, Type> m_viewsRegistry = new Dictionary<string, Type>();
    }
}
