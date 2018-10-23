using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class MyAccountViewModel : BaseViewModel
    {

        public int isSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged(() => isSelected);
            }
        }

        private int tabSelectedIndex;

        public bool IsStaffTabVisible
        {
            get
            {
                if (Global.User != null)
                {
                    if (Global.User.IsStaff)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }

            }
        }

        public bool IsFingerprintTabVisible
        {
            get
            {
                if (Global.EnableFingerprintAuthentication)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }



        public int SelectedIndex
        {
            get
            {
                if (Global.RefillButtonVisible = true )
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
        }



        public bool IsRefillHiddenVisible
        {
            get
            {
                if (Global.RefillButtonVisible)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsHiddenTabVisible
        {
            get
            {
                if (Global.Orientation == "hide")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        

        public int TabSelectedIndex
        {
            get { return tabSelectedIndex; }
            set
            {
                tabSelectedIndex = value;
                RaisePropertyChanged(() => TabSelectedIndex);               
            }
        }

        private bool isTabEnabled;

        public bool IsTabEnabled
        {
            get { return isTabEnabled; }
            set { isTabEnabled = value; RaisePropertyChanged(() => IsTabEnabled); }
        }

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public override void Init()
        {
            aggregator.GetEvent<EventAggregation.FingerPrintCancelEvent>().Subscribe(FingerPrintCancel);
            aggregator.GetEvent<EventAggregation.FingerPrintSaveFailEvent>().Subscribe(FingerPrintCancel);
            aggregator.GetEvent<EventAggregation.FingerPrintSaveSuccessfulEvent>().Subscribe(FingerPrintCancel);
            aggregator.GetEvent<EventAggregation.RightToolBarEnableEvent>().Subscribe(RightToolBarEnable);

            IsTabEnabled = true;
            if (Global.RefillButtonVisible == true) {  TabSelectedIndex = 0; } else { TabSelectedIndex = 6;}            
            base.Init();
        }

        private void RightToolBarEnable(object obj)
        {
            IsTabEnabled = Convert.ToBoolean(obj);
        }

        public void FingerPrintCancel(object parameter = null)
        {
            TabSelectedIndex = 0;
        }

        public override void Dispose()
        {
            aggregator.GetEvent<EventAggregation.FingerPrintCancelEvent>().Unsubscribe(FingerPrintCancel);
            aggregator.GetEvent<EventAggregation.FingerPrintSaveFailEvent>().Unsubscribe(FingerPrintCancel);
            aggregator.GetEvent<EventAggregation.FingerPrintSaveSuccessfulEvent>().Unsubscribe(FingerPrintCancel);

            TabSelectedIndex = 0;
        }
    }
}
