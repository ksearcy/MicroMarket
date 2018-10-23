/*
 * Copyright (c) 2008, Andrzej Rusztowicz (ekus.net)
* All rights reserved.

* Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

* Neither the name of ekus.net nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
/*
 * Added by Michele Cattafesta (mesta-automation.com) 29/2/2011
 * The code has been totally rewritten to create a control that can be modified more easy even without knowing the MVVM pattern.
 * If you need to check the original source code you can download it here: http://wosk.codeplex.com/
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Text.RegularExpressions;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.Views
{
    /// <summary>
    /// Logica di interazione per VirtualKeyboard.xaml
    /// </summary>
    public partial class VirtualKeyboard : Window, INotifyPropertyChanged
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        #region Public Properties

        private bool _showNumericKeyboard;
        public bool ShowNumericKeyboard
        {
            get { return _showNumericKeyboard; }
            set { _showNumericKeyboard = value; this.OnPropertyChanged("ShowNumericKeyboard"); }
        }

        private string _result;
        public string Result
        {
            get { return _result; }
            private set { _result = value; this.OnPropertyChanged("Result"); }
        }

        #endregion

        #region Constructor

        public VirtualKeyboard(TextBox owner, Window wndOwner, string text)
        {
            InitializeComponent();
            this.Owner = wndOwner;
            this.DataContext = this;
            Result = text;

            TextBox_VKeyboard.Text = Result;
            TextBox_VKeyboard.Visibility = System.Windows.Visibility.Visible;

            App.Current.MainWindow.Opacity = 0.3;
        }

        public VirtualKeyboard(PasswordBox owner, Window wndOwner, string text)
        {
            InitializeComponent();
            this.Owner = wndOwner;
            this.DataContext = this;
            Result = text;

            PasswordBox_VKeyboard.Password = Result;
            PasswordBox_VKeyboard.Visibility = System.Windows.Visibility.Visible;

            App.Current.MainWindow.Opacity = 0.3;

        }

        #endregion

        #region Callbacks

        public void Dispose(object parameter = null)
        {

            this.Close();
        
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int index = TextBox_VKeyboard.CaretIndex;

            Button button = sender as Button;
            if (button != null)
            {
                switch (button.CommandParameter.ToString())
                {
                    case "LSHIFT":
                        Regex upperCaseRegex = new Regex("[A-Z]");
                        Regex lowerCaseRegex = new Regex("[a-z]");
                        Button btn;
                        foreach (UIElement elem in AlfaKeyboard.Children) //iterate the main grid
                        {
                            Grid grid = elem as Grid;
                            if (grid != null)
                            {
                                foreach (UIElement uiElement in grid.Children)  //iterate the single rows
                                {
                                    btn = uiElement as Button;
                                    if (btn != null) // if button contains only 1 character
                                    {
                                        if (btn.Content.ToString().Length == 1)
                                        {
                                            if (upperCaseRegex.Match(btn.Content.ToString()).Success) // if the char is a letter and uppercase
                                                btn.Content = btn.Content.ToString().ToLower();
                                            else if (lowerCaseRegex.Match(button.Content.ToString()).Success) // if the char is a letter and lower case
                                                btn.Content = btn.Content.ToString().ToUpper();
                                        }

                                    }
                                }
                            }
                        }
                        break;

                    case "ALT":
                    case "CTRL":
                        break;

                    case "RETURN":
                        this.DialogResult = true;
                        break;

                    case "BACK":
                        if (Result.Length > 0)
                        {
                            if (TextBox_VKeyboard.CaretIndex > 0)
                            {
                                Result = Result.Remove(TextBox_VKeyboard.CaretIndex - 1, 1);
                                TextBox_VKeyboard.CaretIndex = index - 1;
                            }
                        }
                        break;

                    default:
                        if (index != TextBox_VKeyboard.Text.Length)
                        {
                            Result = Result.Insert(index, button.Content.ToString());
                            TextBox_VKeyboard.CaretIndex = index + 1;
                        }
                        else
                        {
                            Result += button.Content.ToString();
                            TextBox_VKeyboard.CaretIndex = Result.Length;
                        }
                        break;
                }
            }
            TextBox_VKeyboard.Focus();
        }

        #endregion

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void Keyboard_Closing(object sender, EventArgs e)
        {
            aggregator.GetEvent<EventAggregation.CloseKeyboardOnTimeOut>().Unsubscribe(Dispose);

            App.Current.MainWindow.Opacity = 1.0;
        }

        private void Keyboard_Loaded(object sender, RoutedEventArgs e)
        {
            aggregator.GetEvent<EventAggregation.CloseKeyboardOnTimeOut>().Subscribe(Dispose);

            if (Helpers.Global.DefaultToNumericKeyPad)
                ShowNumericKeyboard = true;

            custom1.CommandParameter = Helpers.Global.CompanyEmail;
            custom1.Content = Helpers.Global.CompanyEmail;
            custom2.CommandParameter = Helpers.Global.CompanyEmail;
            custom2.Content = Helpers.Global.CompanyEmail;

            TextBox_VKeyboard.Focus();
            TextBox_VKeyboard.CaretIndex = TextBox_VKeyboard.Text.Length;


            
        }
    }
}
