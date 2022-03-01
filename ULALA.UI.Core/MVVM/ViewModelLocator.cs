using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using ULALA.Infrastructure.IOC;
using ULALA.UI.Core.Contracts.MVVM;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ULALA.UI.Core.MVVM
{
    public class ViewModelLocator : DependencyObject
    {
        public static readonly DependencyProperty AutoWireViewModelProperty =
                                        DependencyProperty.RegisterAttached("AutoWireViewModel", typeof(bool), 
                                        typeof(ViewModelLocator), new PropertyMetadata(null, new PropertyChangedCallback(OnAutoWireViewModelChanged)));

        public static bool GetAutoWireViewModel(DependencyObject target)
        {
            return (bool)target.GetValue(AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(DependencyObject target, bool value)
        {
            target.SetValue(AutoWireViewModelProperty, value);

        }
        private static void OnAutoWireViewModelChanged(DependencyObject bindable, DependencyPropertyChangedEventArgs e)
        {
            var view = bindable as Page;
            if (view == null)
                return;

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");

            if ( viewName.EndsWith("Page") )
                viewName = viewName.Replace("Page", "View");

            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType == null)
            {
                return;
            }
            
            var viewModel = (IViewModelBase)ServiceLocator.Current.Resolve(viewModelType);
            view.DataContext = viewModel;

            if (bindable is IParametrizedView paramView)
                BindParametersToViewModel(paramView, viewModel);

            if (bindable is Page page)
            {
                page.Loading += (sender, args) =>
                {
                    viewModel.HandleActivated();
                };

                page.Unloaded += (sender, args) =>
                {
                    viewModel.HandleDeactivated();
                };
            }
        }

        public static void BindParametersToViewModel(IParametrizedView paramView, object viewModel)
        {
            foreach( var param in paramView.Parameters )
            {
                var type = viewModel.GetType();
                var paramProp = type.GetProperty(param.Key);

                if ( paramProp != null )
                {
                    var setMethod = paramProp.GetSetMethod();
                    if ( setMethod != null )
                        setMethod.Invoke(viewModel, new object[] { param.Value });
                }
            }
        }
    }
}
