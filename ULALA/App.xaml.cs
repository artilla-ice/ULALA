using System;
using System.Globalization;
using System.Threading;
using ULALA.Core.Contracts.Base;
using ULALA.Core.Contracts.Zeus;
using ULALA.Core.Zeus;
using ULALA.Infrastructure.IOC;
using ULALA.Infrastructure.PubSub;
using ULALA.Services.Contracts.Zeus;
using ULALA.Services.Zeus;
using ULALA.UI.Core.Contracts.Navigation;
using ULALA.UI.Core.Navigation;
using ULALA.Views;
using Unity;
using Unity.Lifetime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Globalization;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms.Internals;

using Application = Windows.UI.Xaml.Application;
using Frame = Windows.UI.Xaml.Controls.Frame;
using Size = Windows.Foundation.Size;

namespace ULALA
{
    public sealed partial class App : Application, INavigationProxy
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            InitializeConfiguration();
            ConfigureNavigationManager();
            //ConfigureZeusConnectionService();

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainMenuView));
                }

                // Ensure the current window is active
                ConfigureWindowApplication();

                Window.Current.Activate();

                LoadCultureApplication();
            }
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active


            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            InitializeConfiguration();
            ConfigureNavigationManager();
            //ConfigureZeusConnectionService();

            if (args.Kind == ActivationKind.CommandLineLaunch)
            {
                var commandLineArgs = args as CommandLineActivatedEventArgs;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainMenuView));
            }

            // Ensure the current window is active
            ConfigureWindowApplication();

            Window.Current.Activate();

            LoadCultureApplication();
        }

        void LoadCultureApplication()
        {
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            ApplicationLanguages.PrimaryLanguageOverride = "en-US";
            Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Reset();
            Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void InitializeConfiguration()
        {
            if (m_container == null)
            {
                m_container = new UnityContainer();
                ServiceLocator.SetContainer(m_container);

                var eventAggregator = new EventAggregator();
                m_container.RegisterInstance<IEventAggregator>(eventAggregator);

                var m_zeusConnectionService = new ZeusConnectionService();
                m_container.RegisterInstance<IZeusConnectionService>(m_zeusConnectionService);
                m_container.RegisterType<IZeusConnectionService, ZeusConnectionService>(new ContainerControlledLifetimeManager());

                m_container.RegisterType<IZeusManager, ZeusManager>(new ContainerControlledLifetimeManager());
                m_container.RegisterType<ISQLDependencyManager, SQLDependencyManager>(new ContainerControlledLifetimeManager());


                // Como registrar un nuevo Service con su Manager
                //m_container.RegisterType<ITestService, TestService>();
                //m_container.RegisterType<ITestManage, TestManager>(new ContainerControlledLifetimeManager());
            }

        }

        private void InitializeManager<T>()
        {
            var manager = m_container.Resolve<T>();
            var initializable = manager as IInitializableManager;
            if (initializable != null)
                initializable.Initialize().Wait();

        }

        private void InitializeZeusConnection()
        {
            var zeusService = m_container.Resolve<IZeusConnectionService>();
            if (zeusService != null)
                zeusService.Initialize().Wait();
        }

        private void ConfigureNavigationManager()
        {
            var navigationManager = new NavigationManager();
            m_container.BuildUp(navigationManager);
            m_container.RegisterInstance<INavigationManager>(navigationManager);

            RegisterNavigationPages(navigationManager);

            InitializeZeusConnection();
            InitializeManager<ISQLDependencyManager>();
            InitializeManager<IZeusManager>();
        }

        //private void ConfigureDBListener()
        //{
        //    var dbListenerManager = new SQLDependencyManager();
        //    m_container.BuildUp(dbListenerManager);
        //    m_container.RegisterInstance<ISQLDependencyManager>(dbListenerManager);

        //    //dbListenerManager.StartListening();
        //}

        private void RegisterNavigationPages(INavigationManager navigationManager)
        {
            navigationManager.RegisterView(ViewNames.MainMenu, typeof(MainMenuView));
            navigationManager.RegisterView(ViewNames.WithdrawStacker, typeof(WithdrawStackerView));
            navigationManager.RegisterView(ViewNames.CashFunds, typeof(CashFundsView));
            navigationManager.RegisterView(ViewNames.CashOut, typeof(CashOutView));
            navigationManager.RegisterView(ViewNames.Logs, typeof(LogsView));
            navigationManager.RegisterView(ViewNames.AddExchange, typeof(AddExchangeView));
            navigationManager.RegisterView(ViewNames.WithdrawCash, typeof(WithdrawCashView));
            navigationManager.RegisterView(ViewNames.ControlPanel, typeof(ControlPanelView));
            navigationManager.RegisterView(ViewNames.ZeusConnectionSettings, typeof(ZeusConnectionSettingsView));
            navigationManager.RegisterView(ViewNames.NewCharge, typeof(NewChargeView));
        }

        private void ConfigureWindowApplication()
        {
            ApplicationView.PreferredLaunchViewSize = new Size(1024, 770);
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(1024, 770));
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            //Set window title bar transparent
            ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
        }

        public NavigationProxy NavigationProxy { get; }
        private UnityContainer m_container;
    }
}
