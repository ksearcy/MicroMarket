using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using DPUruNet;
using Microsoft.Practices.Composite.Events;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;

namespace deORO.ViewModels
{
    class FingerPrintViewModel : BaseViewModel
    {
        private deOROMembershipProvider membership = new deOROMembershipProvider();

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        //public ICommand FingerPrintsCommand { get { return new DelegateCommandWithParam(ExecuteFingerPrintsCommand, () => { return canExecuteSave; }); } }
        public ICommand FingerPrintsCommand { get { return new DelegateCommandWithParam(ExecuteFingerPrintsCommand); } }
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }
        //IDialogService dialog = new MessageBoxViewService();

        private bool canExecuteSave;
        public bool CanExecuteSave
        {
            get { return canExecuteSave; }
            set
            {
                canExecuteSave = value;
                RaisePropertyChanged(() => CanExecuteSave);
            }
        }
        
        private void ExecuteFingerPrintsCommand(object parameter)
        {

            bool updated = membership.UpdateFingerprints(Global.User, parameter as Dictionary<int, DataResult<Fmd>>);
            //Global.Email.SendFingerPrintChanged();

            if (updated)
            {
                DialogViewService.ShowAutoCloseDialog("Enroll Fingerprint", "Fingerprint Saved Successfully");
                aggregator.GetEvent<EventAggregation.FingerPrintSaveSuccessfulEvent>().Publish(null);
            }
            else
            {
                DialogViewService.ShowAutoCloseDialog("Enroll Fingerprint", "Failed to Save Fingerprint");
                aggregator.GetEvent<EventAggregation.FingerPrintSaveFailEvent>().Publish(null);
            }
            
        }

        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.FingerPrintCancelEvent>().Publish(null);
        }

        public override void Init()
        {
            //CanExecuteSave = true;
            base.Init();
        }
        private bool fingerImage;

        public bool FingerImage
        {
            get { return fingerImage; }
            set { fingerImage = value; RaisePropertyChanged(() => FingerImage); }
        }
    }
}
