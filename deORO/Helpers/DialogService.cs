using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using deORO.EventAggregation;
using deORO.ViewModels;
using deORO.Views;
using Microsoft.Practices.Composite.Events;
using System.Windows.Shell;
using System.Windows;

namespace deORO.Helpers
{
    public interface IDialogService
    {
        int Width { get; set; }
        int Height { get; set; }
        bool CancelDialog { get; set; }
        void Show(string title, string message, Action<DialogResult> onClosedCallback = null);
        void Show(BaseViewModel vm);
        void ShowDialog(string title, string message);
        void ShowDialog(BaseViewModel vm);
        void ShowDialog(BaseViewModel vm, bool showClose);
        void ShowAutoCloseDialog(string title, string message);
        void ShowAutoCloseDialog(BaseViewModel vm);
    }

    public class DialogResult
    {
        public bool? Result { get; set; }
    }


    public static class DialogViewService
    {
        static IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public static void Show(string title, string message, Action<DialogResult> onClosedCallback, int height = 400, int width = 400)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                MessageBoxView view = new MessageBoxView(title, message);
                view.Height = height;
                view.Width = width;

                view.Closed += (s, e) =>
                {
                    if (onClosedCallback != null)
                    {
                        DialogResult result = new DialogResult();
                        result.Result = view.DialogResult;
                        onClosedCallback(result);
                    }
                };

                view.PrintButton.Visibility = System.Windows.Visibility.Collapsed;
                view.Owner = System.Windows.Application.Current.MainWindow;
                view.ShowDialog();
                System.Threading.Thread.Sleep(500);
                view.Activate();
            });

        }

        //public static void Window_Deactivated(object sender, EventArgs e)
        //{
        //    // The Window was deactivated 
        //    if (!Topmost && !confirmActive)
        //    {
        //        View.Close();
        //    }
        //}

        public static void Show(BaseViewModel vm, int width = 400, int height = 400)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                PopupView view = new PopupView();
                view.Height = height;
                view.Width = width;

                view.MainContent.Content = vm;
                view.Owner = System.Windows.Application.Current.MainWindow;
                view.Show();
                System.Threading.Thread.Sleep(500);
                view.Activate();
            });
        }

        public static void ShowDialog(string title, string message, int width = 400, int height = 400)
        {
            if (Global.DisablePopups && (message.Contains("Purchase Complete") || message.Contains("Invalid Fingerprint") || message.Contains("Login Failed")) || message.Contains("Fingerprint") || message.Contains("Account refill was successful.") || message.Contains("barcode does not exist"))
            {

                if (message.Contains("Purchase Complete") || message.Contains("Account refill was successful.") || message.Contains("Fingerprint Saved Successfully"))
                {
                    Global.DynamicPanelDialogType = "Succsessful";
                }
                else
                {

                    Global.DynamicPanelDialogType = "Fail";
                }

                if (message.Contains("Purchase Complete"))
                {
                    Global.DynamicPanelDialogTitle = LocalizationProvider.GetLocalizedValue<string>("Message.PurchaseComplete");
                    Global.DynamicPanelDialogMessage = LocalizationProvider.GetLocalizedValue<string>("Message.PurchaseComplete");


                }
                else if (message.Contains("Invalid Fingerprint"))
                {
                    Global.DynamicPanelDialogTitle = LocalizationProvider.GetLocalizedValue<string>("Message.InvalidFingerprintTitle");
                    Global.DynamicPanelDialogMessage = LocalizationProvider.GetLocalizedValue<string>("Message.InvalidFingerprintMessage");
                }
                else if (message.Contains("Login Failed"))
                {
                    Global.DynamicPanelDialogTitle = LocalizationProvider.GetLocalizedValue<string>("Message.LoginFailedTitle");
                    Global.DynamicPanelDialogMessage = LocalizationProvider.GetLocalizedValue<string>("Message.LoginFailedMessage");
                }
                else if (message.Contains("Account refill was successful."))
                {
                    Global.DynamicPanelDialogTitle = LocalizationProvider.GetLocalizedValue<string>("Message.SuccessfulRefillTitle");
                    Global.DynamicPanelDialogMessage = LocalizationProvider.GetLocalizedValue<string>("Message.SuccessfulRefillMessage");
                 
                }
                else if (message.Contains("Fingerprint Saved Successfully"))
                {
                    Global.DynamicPanelDialogTitle = LocalizationProvider.GetLocalizedValue<string>("Message.FingerPrintRegistrationTtile");
                    Global.DynamicPanelDialogMessage = LocalizationProvider.GetLocalizedValue<string>("Message.SuccessfulFingerPrintRegistrationMessage");

                }
                else if (message.Contains("Failed to Save Fingerprint"))
                {
                    Global.DynamicPanelDialogTitle = LocalizationProvider.GetLocalizedValue<string>("Message.FingerPrintRegistrationTtile");
                    Global.DynamicPanelDialogMessage = LocalizationProvider.GetLocalizedValue<string>("Message.UnsuccessfulFingerPrintRegistrationMessage");

                }
                else
                {

                    Global.DynamicPanelDialogTitle = title;
                    Global.DynamicPanelDialogMessage = message;

                }





                aggregator.GetEvent<EventAggregation.ShowDynamicPanelDialog>().Publish(null);


            }
            else
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageBoxView view = new MessageBoxView(title, message, true);
                    view.Height = height;
                    view.Width = width;

                    view.CancelButton.Visibility = System.Windows.Visibility.Collapsed;
                    view.PrintButton.Visibility = System.Windows.Visibility.Collapsed;
                    view.Owner = System.Windows.Application.Current.MainWindow;
                    view.ShowDialog();
                    System.Threading.Thread.Sleep(500);
                    view.Activate();

                });
            }

        }

        public static void ShowDialog(BaseViewModel vm, int width = 400, int height = 400)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                PopupView view = new PopupView();
                view.Height = height;
                view.Width = width;

                view.MainContent.Content = vm;
                view.Owner = System.Windows.Application.Current.MainWindow;
                view.ShowDialog();
                System.Threading.Thread.Sleep(500);
                view.Activate();
            });
        }

        public static void ShowDialog(BaseViewModel vm, bool showClose, int width = 400, int height = 400)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                PopupView view = new PopupView();
                view.Height = height;
                view.Width = width;

                if (showClose)
                {
                    view.Row1.Height = new System.Windows.GridLength(1.0, System.Windows.GridUnitType.Star);
                    view.Row2.Height = new System.Windows.GridLength(100, System.Windows.GridUnitType.Pixel);
                }
                else
                {
                    view.Row1.Height = new System.Windows.GridLength(1.0, System.Windows.GridUnitType.Star);
                    view.Row2.Height = new System.Windows.GridLength(0.0, System.Windows.GridUnitType.Star);
                }

                view.MainContent.Content = vm;
                view.Owner = System.Windows.Application.Current.MainWindow;
                view.ShowDialog();
                System.Threading.Thread.Sleep(500);
                view.Activate();
            });
        }

        public static void ShowAutoCloseDialog(string title, string message, int width = 400, int height = 400)
        {
            if (Global.DisablePopups && (message.Contains("Purchase Complete") || message.Contains("Invalid Fingerprint") || message.Contains("Login Failed")) || message.Contains("Fingerprint") || message.Contains("Account refill was successful.") || message.Contains("barcode does not exist"))
            {
               
                if (message.Contains("Purchase Complete") || message.Contains("Account refill was successful.") || message.Contains("Fingerprint Saved Successfully"))
                {
                    Global.DynamicPanelDialogType = "Succsessful";
                }
                else
                {

                    Global.DynamicPanelDialogType = "Fail";
                }

                if (message.Contains("Purchase Complete"))
                {
                    Global.DynamicPanelDialogTitle = LocalizationProvider.GetLocalizedValue<string>("Message.PurchaseComplete");
                    Global.DynamicPanelDialogMessage = LocalizationProvider.GetLocalizedValue<string>("Message.PurchaseComplete");

                }
                else if (message.Contains("Invalid Fingerprint"))
                {
                    Global.DynamicPanelDialogTitle = LocalizationProvider.GetLocalizedValue<string>("Message.InvalidFingerprintTitle");
                    Global.DynamicPanelDialogMessage = LocalizationProvider.GetLocalizedValue<string>("Message.InvalidFingerprintMessage");
                }
                else if (message.Contains("Login Failed"))
                {
                    Global.DynamicPanelDialogTitle = LocalizationProvider.GetLocalizedValue<string>("Message.LoginFailedTitle");
                    Global.DynamicPanelDialogMessage = LocalizationProvider.GetLocalizedValue<string>("Message.LoginFailedMessage");
                }
                else if (message.Contains("Account refill was successful."))
                {
                    Global.DynamicPanelDialogTitle = LocalizationProvider.GetLocalizedValue<string>("Message.SuccessfulRefillTitle");
                    Global.DynamicPanelDialogMessage = LocalizationProvider.GetLocalizedValue<string>("Message.SuccessfulRefillMessage");
                

                }
                else if (message.Contains("Fingerprint Saved Successfully"))
                {
                    Global.DynamicPanelDialogTitle = LocalizationProvider.GetLocalizedValue<string>("Message.FingerPrintRegistrationTtile");
                    Global.DynamicPanelDialogMessage = LocalizationProvider.GetLocalizedValue<string>("Message.SuccessfulFingerPrintRegistrationMessage");

                }
                else if (message.Contains("Failed to Save Fingerprint"))
                {
                    Global.DynamicPanelDialogTitle = LocalizationProvider.GetLocalizedValue<string>("Message.FingerPrintRegistrationTtile");
                    Global.DynamicPanelDialogMessage = LocalizationProvider.GetLocalizedValue<string>("Message.UnsuccessfulFingerPrintRegistrationMessage");

                }
                else
                {

                    Global.DynamicPanelDialogTitle = title;
                    Global.DynamicPanelDialogMessage = message;

                }





                aggregator.GetEvent<EventAggregation.ShowDynamicPanelDialog>().Publish(null);


            }
            else
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageBoxView view = new MessageBoxView(title, message, true);
                    view.Height = height;
                    view.Width = width;

                    view.CancelButton.Visibility = System.Windows.Visibility.Collapsed;
                    view.PrintButton.Visibility = System.Windows.Visibility.Collapsed;
                    view.Owner = System.Windows.Application.Current.MainWindow;
                    view.ShowDialog();
                    System.Threading.Thread.Sleep(500);
                    view.Activate();

                });
            }

        }



        public static void ShowAutoCloseDialog(BaseViewModel vm, int width = 400, int height = 400, string timerType = "message")
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                PopupView view = new PopupView(true, timerType);
                view.Height = height;
                view.Width = width;

                view.MainContent.Content = vm;
                view.Owner = System.Windows.Application.Current.MainWindow;
                view.ShowDialog();
                System.Threading.Thread.Sleep(500);
                view.Activate();
            });
        }
    }



    //public class PopupViewService : IDialogService
    //{
    //    public int Width { get; set; }
    //    public int Height { get; set; }
    //    public bool CancelDialog { get; set; }

    //    readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

    //    public PopupViewService(int width, int height)
    //    {
    //        this.Width = width;
    //        this.Height = height;
    //        CancelDialog = false;
    //    }

    //    public void Show(string title, string message, Action<DialogResult> onClosedCallback = null)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Show(BaseViewModel vm)
    //    {
    //        App.Current.Dispatcher.Invoke((Action)delegate
    //        {
    //            PopupView view = new PopupView();
    //            view.Height = this.Height;
    //            view.Width = this.Width;

    //            view.MainContent.Content = vm;
    //            view.Show();
    //        });
    //    }

    //    public void ShowDialog(BaseViewModel vm)
    //    {
    //        aggregator.GetEvent<EventAggregation.PopupCancelEvent>().Subscribe((x) => { CancelDialog = true; });

    //        App.Current.Dispatcher.Invoke((Action)delegate
    //       {
    //           PopupView view = new PopupView();
    //           view.Height = this.Height;
    //           view.Width = this.Width;

    //           view.MainContent.Content = vm;
    //           view.ShowDialog();
    //       });
    //    }


    //    public void ShowDialog(BaseViewModel vm, bool showClose)
    //    {
    //        aggregator.GetEvent<EventAggregation.PopupCancelEvent>().Subscribe((x) => { CancelDialog = true; });

    //        App.Current.Dispatcher.Invoke((Action)delegate
    //        {
    //            PopupView view = new PopupView();
    //            view.Height = this.Height;
    //            view.Width = this.Width;

    //            if (showClose)
    //            {
    //                view.Row1.Height = new System.Windows.GridLength(1.0, System.Windows.GridUnitType.Star);
    //                view.Row2.Height = new System.Windows.GridLength(100, System.Windows.GridUnitType.Pixel);
    //            }
    //            else
    //            {
    //                view.Row1.Height = new System.Windows.GridLength(1.0, System.Windows.GridUnitType.Star);
    //                view.Row2.Height = new System.Windows.GridLength(0.0, System.Windows.GridUnitType.Star);
    //            }

    //            view.MainContent.Content = vm;
    //            view.ShowDialog();
    //        });
    //    }

    //    public void ShowAutoCloseDialog(BaseViewModel vm)
    //    {
    //        aggregator.GetEvent<EventAggregation.PopupCancelEvent>().Subscribe((x) => { CancelDialog = true; });

    //        App.Current.Dispatcher.Invoke((Action)delegate
    //        {
    //            PopupView view = new PopupView(true);
    //            view.Height = this.Height;
    //            view.Width = this.Width;

    //            view.MainContent.Content = vm;
    //            view.ShowDialog();
    //        });
    //    }

    //    public void ShowAutoCloseDialog(string title, string message)
    //    {
    //        throw new NotImplementedException();
    //    }


    //    public void ShowDialog(string title, string message)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class MessageBoxViewService : IDialogService
    //{
    //    public int Width { get; set; }
    //    public int Height { get; set; }
    //    public bool CancelDialog { get; set; }

    //    readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

    //    public void Show(string title, string message, Action<DialogResult> onClosedCallback)
    //    {
    //        App.Current.Dispatcher.Invoke((Action)delegate
    //        {
    //            MessageBoxView view = new MessageBoxView(title, message);

    //            view.Closed += (s, e) =>
    //            {
    //                if (onClosedCallback != null)
    //                {
    //                    DialogResult result = new DialogResult();
    //                    result.Result = view.DialogResult;
    //                    onClosedCallback(result);
    //                }
    //            };

    //            view.ShowDialog();
    //        });
    //    }

    //    public void Show(BaseViewModel vm)
    //    {
    //        throw new NotImplementedException();
    //    }


    //    public void ShowDialog(BaseViewModel vm)
    //    {
    //        throw new NotImplementedException();
    //    }


    //    public void ShowDialog(BaseViewModel vm, bool showClose)
    //    {
    //        throw new NotImplementedException();
    //    }


    //    public void ShowAutoCloseDialog(BaseViewModel vm)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void ShowAutoCloseDialog(string title, string message)
    //    {
    //        App.Current.Dispatcher.Invoke((Action)delegate
    //        {
    //            MessageBoxView view = new MessageBoxView(title, message, true);
    //            view.CancelButton.Visibility = System.Windows.Visibility.Collapsed;
    //            view.ShowDialog();
    //        });
    //    }

    //    public void ShowDialog(string title, string message)
    //    {
    //        App.Current.Dispatcher.Invoke((Action)delegate
    //        {
    //            MessageBoxView view = new MessageBoxView(title, message);
    //            view.CancelButton.Visibility = System.Windows.Visibility.Collapsed;
    //            view.ShowDialog();
    //        });
    //    }
    //}

}
