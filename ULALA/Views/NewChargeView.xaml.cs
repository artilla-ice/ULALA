using System.Collections.Generic;
using ULALA.UI.Core.Contracts.MVVM;
using ULALA.UI.Core.MVVM;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ULALA.Views
{
    public partial class NewChargeView : Page, IParametrizedView
    {
        public NewChargeView()
        {
            this.Parameters = new Dictionary<string, object>();
            
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Parameters = (IDictionary<string, object>)e.Parameter;
            ViewModelLocator.BindParametersToViewModel(this, this.DataContext);
        }

        public IDictionary<string, object> Parameters { get; private set; }
    }
}
