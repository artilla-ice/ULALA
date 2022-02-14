using System.Collections.Generic;
using System;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System.Windows.Input;
using System.Globalization;

namespace ULALA.UI.Core.Controls
{

    public partial class NumericVirtualKeyboard : UserControl
    {
        public NumericVirtualKeyboard()
        {
            this.InitializeComponent();
        }

        private void OnReceivingInputKeyboard(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var pressedButton = sender as Windows.UI.Xaml.Controls.Button;
            if (pressedButton.Name == "BackspaceButton" && Value.Length > 0)
            {
                if (this.Value[Value.Length - 1] == '.')
                {
                    this.Value = this.Value.Remove(this.Value.Length - 1);
                    m_firstDecimalIsZero = false;
                }

                this.Value = this.Value.Remove(this.Value.Length - 1);

                return;
            }
            else if (pressedButton.Name == "ReturnButton")
            {
                if (this.ReturnCommand != null)
                {
                    ReturnCommand.CanExecute(true);
                    ReturnCommand.Execute(null);

                    return;
                }

            }

            double pressedDigit = -1;
            var buttonValue = pressedButton.Content.ToString();
            if (buttonValue == "AC")
            {
                this.Value = string.Empty;
                return;
            }

            if (buttonValue == "." && !Value.Contains('.'))
            {
                this.Value += buttonValue;
                return;
            }

            if (!string.IsNullOrEmpty(buttonValue) && char.IsDigit(buttonValue[0]))
            {
                pressedDigit = double.Parse(buttonValue, CultureInfo.InvariantCulture);

                if (Value != null && Value.Contains("."))
                {
                    var pointIndex = Value.IndexOf('.');
                    if ((Value.Length) >= pointIndex)
                    {
                        if ((Value.Length - (pointIndex+1)) == 2)
                            return;

                        var firstDecimalIndex = ++pointIndex;
                        var secondDecimalIndex = ++pointIndex;

                        if ((Value.Length - 1) >= firstDecimalIndex)
                        {
                            var firstDecimal = Value[firstDecimalIndex];
                            if (firstDecimal == '0' && !m_firstDecimalIsZero)
                            {
                                m_firstDecimalIsZero = pressedDigit.ToString() == "0";
                                if (m_firstDecimalIsZero)
                                    return;

                                this.Value = this.Value.Remove(firstDecimalIndex, 1).Insert(firstDecimalIndex, pressedDigit.ToString());

                                return;
                            }

                        }
                        else if (pressedDigit.ToString() == "0")
                        {
                            m_firstDecimalIsZero = true;
                            this.Value += pressedDigit;

                            return;
                        }
                        else if ((Value.Length - 1) >= secondDecimalIndex)
                        {
                            var secondDecimal = Value[secondDecimalIndex];
                            if (secondDecimal == '0')
                                this.Value = this.Value.Remove(secondDecimalIndex).Insert(secondDecimalIndex, pressedDigit.ToString());

                            return;
                        }
                    }
                }

                this.Value += pressedDigit;
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string),
                                                                                    typeof(NumericVirtualKeyboard), null);
        public string Value
        {
            set => SetValue(ValueProperty, value);
            get => (string)GetValue(ValueProperty);
        }

        public static readonly DependencyProperty ReturnCommandProperty = DependencyProperty.Register("ReturnCommand", typeof(ICommand),
                                                                                    typeof(NumericVirtualKeyboard), null);
        public ICommand ReturnCommand
        {
            set => SetValue(ReturnCommandProperty, value);
            get => (ICommand)GetValue(ReturnCommandProperty);
        }

        private bool m_firstDecimalIsZero = false;
    }
}