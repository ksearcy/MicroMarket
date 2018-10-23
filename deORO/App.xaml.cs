using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using deORODataAccessApp.DataAccess;
using deORO.Helpers;
using deORO.Views;
using NLog;
using NLog.Config;
using System.Windows.Threading;
using deORO.ViewModels;
using deORO.CardProcessor;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.Composite.Events;
using deORODataAccessApp;
using Hid.Win32;
using Microsoft.Win32.SafeHandles;
using System.Security;
using System.Runtime.ConstrainedExecution;

namespace deORO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Logger logger = null;
        LoggingConfiguration config = new LoggingConfiguration();
        deORO.Communication.ICommunicationType commType;
        deORO.CardReader.ICardReader cardReader;
        deORO.BarcodeScanner.IBarcodeScanner barcodeScanner;

        public App()
        {
            Global.Init();

            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            App.Current.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            logger = LogManager.GetCurrentClassLogger();

            LogManager.Configuration = config;

            Helpers.NLogConfig.CreateMailTarget(config);
            Helpers.NLogConfig.CreateFileTarget(config);

            commType = deORO.Communication.CommunicationTypeFactory.GetCommunicationType();
            System.Threading.Thread.Sleep(1000);
            commType.InitCoin();
            System.Threading.Thread.Sleep(1000);
            commType.InitBill();
            System.Threading.Thread.Sleep(1000);

            barcodeScanner = BarcodeScanner.BarcodeScannerFactory.GetBarcodeScanner();

            //var section = ConfigurationManager.GetSection("application") as NameValueCollection;
            //EncryptAppSettings("application");
            
            if (Global.EnableVirtualKeyboard)
            {
                EventManager.RegisterClassHandler(typeof(TextBox), TextBox.PreviewMouseUpEvent, new RoutedEventHandler(TextBox_PreviewMouseUpEvent));
                EventManager.RegisterClassHandler(typeof(PasswordBox), PasswordBox.PreviewMouseUpEvent, new RoutedEventHandler(PasswordBox_PreviewMouseUp));
            }

            this.Exit += App_Exit;
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("en");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Style dpStyle = new Style(typeof(System.Windows.Controls.DatePicker));
            dpStyle.Setters.Add(new Setter(System.Windows.Controls.DatePicker.LanguageProperty, System.Windows.Markup.XmlLanguage.GetLanguage("en-US")));
            this.Resources.Add(typeof(System.Windows.Controls.DatePicker), dpStyle);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.ExceptionObject as Exception;
                StringBuilder sb = new StringBuilder();

                if (ex.InnerException != null)
                {
                    sb.AppendLine("InnerException");
                    sb.AppendLine(ex.InnerException.Message);
                    if (!string.IsNullOrEmpty(ex.InnerException.StackTrace))
                    {
                        int count = 0;
                        foreach (string line in ex.InnerException.StackTrace.Split('\n'))
                        {
                            sb.AppendLine(line.Trim());
                            count++;
                            if (count > 3) break;
                        }
                    }
                }
                sb.AppendLine("OuterException");
                sb.AppendLine(ex.Message);
                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    int count = 0;
                    foreach (string line in ex.StackTrace.Split('\n'))
                    {
                        sb.AppendLine(line.Trim());
                        count++;
                        if (count > 3) break;
                    }
                }

                logger.Error(sb.ToString());
                logger.Error("Shutting Down");
                App.Current.Shutdown();
            }
            catch
            {
                logger.Error("Shutting Down");
                App.Current.Shutdown();
            }
        }

        void App_Exit(object sender, ExitEventArgs e)
        {
            Dispose();
        }

        private void Dispose()
        {
            commType.Dispose();
            barcodeScanner.Dispose();

            try
            {
                cardReader = CardReader.CardReaderFactory.GetCreditCardReader();
                cardReader.Dispose();
            }
            catch { }
        }


        private void PasswordBox_PreviewMouseUp(object sender, RoutedEventArgs e)
        {
            PasswordBox source = sender as PasswordBox;

            if (source.Name == "PasswordBox_VKeyboard") return;

            VirtualKeyboard keyBoard = new VirtualKeyboard(source, this.MainWindow, source.Password);
            if (keyBoard.ShowDialog() == true)
                source.Password = keyBoard.Result;
        }


        private void TextBox_PreviewMouseUpEvent(object sender, RoutedEventArgs e)
        {
            TextBox source = sender as TextBox;

            if (source.Name == "TextBox_VKeyboard") return;

            if (source.Tag != null && source.Tag.ToString() == "Number")
            {
                Keypad keyPad = new Keypad(source, this.MainWindow, source.Text);
                if (keyPad.ShowDialog() == true)
                    source.Text = keyPad.Result;
            }
            else if (source.Tag != null && source.Tag.ToString() == "Date")
            {
                KeypadDate keyPadDate = new KeypadDate(source, this.MainWindow, source.Text);
                if (keyPadDate.ShowDialog() == true)
                    source.Text = keyPadDate.Result;
            }
            else
            {
                VirtualKeyboard keyBoard = new VirtualKeyboard(source, this.MainWindow, source.Text);
                if (keyBoard.ShowDialog() == true)
                    source.Text = keyBoard.Result;
            }

        }

        void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

            try
            {
                StringBuilder sb = new StringBuilder();
                if (e.Exception.InnerException != null)
                {
                    sb.AppendLine("InnerException");
                    sb.AppendLine(e.Exception.InnerException.Message);
                    if (!string.IsNullOrEmpty(e.Exception.InnerException.StackTrace))
                    {
                        int count = 0;
                        foreach (string line in e.Exception.InnerException.StackTrace.Split('\n'))
                        {
                            sb.AppendLine(line.Trim());
                            count++;
                            if (count > 3) break;
                        }
                    }
                }
                sb.AppendLine("OuterException");
                sb.AppendLine(e.Exception.Message);
                if (!string.IsNullOrEmpty(e.Exception.StackTrace))
                {
                    int count = 0;
                    foreach (string line in e.Exception.StackTrace.Split('\n'))
                    {
                        sb.AppendLine(line.Trim());
                        count++;
                        if (count > 3) break;
                    }
                }

                logger.Error(sb.ToString());
                e.Handled = true;

                Dispose();

                logger.Error("Shutting Down");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch
            {
                Dispose();

                logger.Error("Shutting Down");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

            try
            {
                StringBuilder sb = new StringBuilder();
                if (e.Exception.InnerException != null)
                {
                    sb.AppendLine("InnerException");
                    sb.AppendLine(e.Exception.InnerException.Message);
                    if (!string.IsNullOrEmpty(e.Exception.InnerException.StackTrace))
                    {
                        int count = 0;
                        foreach (string line in e.Exception.InnerException.StackTrace.Split('\n'))
                        {
                            sb.AppendLine(line.Trim());
                            count++;
                            if (count > 3) break;
                        }
                    }
                }
                sb.AppendLine("OuterException");
                sb.AppendLine(e.Exception.Message);
                if (!string.IsNullOrEmpty(e.Exception.StackTrace))
                {
                    int count = 0;
                    foreach (string line in e.Exception.StackTrace.Split('\n'))
                    {
                        sb.AppendLine(line.Trim());
                        count++;
                        if (count > 3) break;
                    }
                }

                logger.Error(sb.ToString());
                e.Handled = true;

                Dispose();

                logger.Error("Shutting Down");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch
            {
                Dispose();

                logger.Error("Shutting Down");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }

    

}
