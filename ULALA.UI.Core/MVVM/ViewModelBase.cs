using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using ULALA.UI.Core.Contracts.MVVM;

namespace ULALA.UI.Core.MVVM
{
    public class ViewModelBase : IViewModelBase, INotifyPropertyChanged, INotifyDataErrorInfo
    {
       

        bool m_isBusy = false;
        [Display(AutoGenerateField = false)]
        public bool IsBusy
        {
            get { return m_isBusy; }
            set { SetProperty(ref m_isBusy, value); }
        }

        string m_Title = string.Empty;
        [Display(AutoGenerateField = false)]
        public string Title
        {
            get { return m_Title; }
            set { SetProperty(ref m_Title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,[CallerMemberName] string propertyName = "",Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }


        public void HandleActivated()
        {
            OnActivated();
        }

        public void HandleDeactivated()
        {
            OnDeactivated();
        }

        protected virtual void OnActivated()
        {
        }
        protected virtual void OnDeactivated()
        {
        }

        protected async void HandleAsyncCall(Func<Task> asynchCallback, bool setIsBusy = true, Action<Exception> onErrorCallback = null)
        {
            await InternalHandleAsyncCall(asynchCallback, setIsBusy, onErrorCallback);
        }

        protected Task WrapAsyncCall(Func<Task> asynchCallback, bool setIsBusy = true, Action<Exception> onErrorCallback = null)
        {
            return InternalHandleAsyncCall(asynchCallback, setIsBusy, onErrorCallback);
        }

        private async Task InternalHandleAsyncCall(Func<Task> asynchCallback, bool setIsBusy, Action<Exception> onErrorCallback)
        {
            if (setIsBusy)
            {
                SetIsBusy(true);
            }

            try
            {
                await asynchCallback();
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex);    

                if ( onErrorCallback != null )
                    onErrorCallback(ex);

                await DisplayUnexpectedErrorMessage(ex);
            }
            finally
            {
                if (setIsBusy)
                    SetIsBusy(false);
            }
        }

        private void SetIsBusy(bool isBusy)
        {
            if (isBusy)
            {
                if (m_isBusyCounter == 0)
                    this.IsBusy = true;
                m_isBusyCounter++;
            }
            else
            {
                if (m_isBusyCounter > 0)
                {
                    m_isBusyCounter--;
                    if (m_isBusyCounter == 0)
                        this.IsBusy = false;
                }
            }

        }

        protected void LogErrorMessage(Exception ex)
        {
            System.Diagnostics.Trace.WriteLine("Exception Error Message: {0}", ex.Message);
            System.Diagnostics.Trace.WriteLine("Exception Error Stack Trace: {0}", ex.StackTrace);
        }

        protected async Task DisplayUnexpectedErrorMessage(Exception ex)
        {
            Exception curEx = ex;
            var exMsgs = new StringBuilder();
            int i = 0;
            do
            {
                if ( curEx != null )
                    exMsgs.AppendLine($"({++i}) {curEx.Message}");
                else
                    break;

                curEx = curEx.InnerException;
            } while ( curEx != null );
                
            //TODO: Crear dialogo para mostrar excepciones generales
        }

        protected Tuple<string, object> NavArg(string name, object val)
        {
            return new Tuple<string, object>(name, val);
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

        #region INotifyDataErrorInfo
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void OnErrorsChanged([CallerMemberName] string propertyName = "")
        {
            var changed = ErrorsChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        [Display(AutoGenerateField = false)]
        public bool HasErrors
        {
            get
            {
                return false;
            }
        }


        public IEnumerable GetErrors(string propertyName)
        {
            var list = new List<object>();
            //if (!propertyName.Equals("Title"))
            //    return list;

            //if (this.Title.Contains("Marketing"))
            //    list.Add("Marketing is not allowed");
            return list;
        }
        #endregion

        private int m_isBusyCounter;
    }
}
