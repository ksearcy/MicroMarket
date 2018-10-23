using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using deORODataAccessApp;
using deORODataAccessApp.Models;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class FastTouchViewModel : BaseViewModel
    {
        FastTouchRepository repo = new FastTouchRepository();

        public ICommand PreviousPageCommand { get { return new DelegateCommand(ExecutePreviousPageCommand, CanExecutePreviousPageCommand); } }
        public ICommand NextPageCommand { get { return new DelegateCommand(ExecuteNextPageCommand, CanExecuteNextPageCommand); } }
        public ICommand ItemSelectCommand { get { return new DelegateCommandWithParam(ExecuteItemSelectCommand); } }
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand DamageBarCodeCommand { get { return new DelegateCommand(ExecuteDamageBarCodeCommand); } }


        List<FastTouch> items;
        public List<FastTouch> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged(() => Items); }
        }

        private int currentPage = 1;
        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                currentPage = value;
                RaisePropertyChanged(() => CurrentPage);
            }
        }

        private int itemCount = 0;
        private const int pageSize = 4;
        private int availablePages = 0;

        public override void Init()
        {
            try
            {
                Items = repo.GetFastTouchItems(CurrentPage, pageSize);
                itemCount = repo.GetFastTouchItemCount();
                availablePages = Convert.ToInt32(Math.Ceiling((decimal)itemCount / (decimal)pageSize));
            }
            catch { }

            base.Init();
        }

        private void ExecutePreviousPageCommand()
        {
            CurrentPage--;
            Items = repo.GetFastTouchItems(CurrentPage, pageSize);
        }

        private void ExecuteNextPageCommand()
        {
            CurrentPage++;
            Items = repo.GetFastTouchItems(CurrentPage, pageSize);
        }

        private void ExecuteItemSelectCommand(object obj)
        {
            aggregator.GetEvent<EventAggregation.BarcodeScannerDataGlobalEvent>().Publish(obj);
            aggregator.GetEvent<EventAggregation.FastTouchItemSelectedEvent>().Publish(obj);
          
        }

        private bool CanExecuteNextPageCommand()
        {
            if (Items == null || CurrentPage == availablePages)
                return false;

            if (itemCount > pageSize)
                return true;
            else
                return false;
        }

        private bool CanExecutePreviousPageCommand()
        {
            if (CurrentPage == 1)
                return false;
            else
                return true;
        }

        private bool damageBarCodeVisible = true;
        public bool DamageBarCodeVisible
        {
            get { return damageBarCodeVisible; }
            set
            {
                damageBarCodeVisible = value;
                RaisePropertyChanged(() => DamageBarCodeVisible);
            }
        }

        

        private void ExecuteDamageBarCodeCommand()
        {
            DamagedBarcodeViewModel viewModel = new DamagedBarcodeViewModel();
            DialogViewService.ShowAutoCloseDialog(viewModel, 800, 600);
        }
        private bool damageBarCodeNewVisible = true;
        public bool DamageBarCodeNewVisible
        {
            get { return damageBarCodeNewVisible; }
            set
            {
                damageBarCodeNewVisible = value;
                RaisePropertyChanged(() => DamageBarCodeNewVisible);
            }
        }



        private void ExecuteDamageBarNewCodeCommand()
        {
            DamagedBarcodeViewModel viewModel = new DamagedBarcodeViewModel();
        }

    }
}
