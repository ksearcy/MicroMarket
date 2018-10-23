using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using deORO.Helpers;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORODataAccessApp.Models;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class DamagedBarcodeViewModel : BaseViewModel
    {

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand AddToCartCommand { get { return new DelegateCommand(ExecuteAddToCartCommand, CanExecuteAddToCartCommand); } }
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommandFinish); } }
        //public ICommand CancelCommand { get { return new DelegateCommand(() => { aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null); }); } }
        //public ICommand CancelCommandFinish { get { return new DelegateCommand(ExecuteCancelCommandFinish); } }

        deOROEntities entities = new deOROEntities();
        ItemRepository repo = new ItemRepository();

        List<DamagedItem> items;

        private int tabSelectedIndex;
        private bool isTabEnabled;

        public bool IsTabEnabled
        {
            get { return isTabEnabled; }
            set { isTabEnabled = value; RaisePropertyChanged(() => IsTabEnabled); }
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

        public int isSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged(() => isSelected);
            }
        }

        public List<DamagedItem> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged(() => Items); }
        }

        private void ExecuteCancelCommandFinish()
        {
            CancelButtonEnabled = false;
            aggregator.GetEvent<EventAggregation.MissingBarcodeItemAddToCartCancelEvent>().Publish(null);
            base.Dispose();

        }

        string filterText;

        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                RaisePropertyChanged(() => FilterText);
                ApplyFilter();
            }
        }

        private void ApplyFilter()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Items);

            if (view != null)
            {
                view.Filter = ((x) =>
                {
                    if (FilterText == "") return true;

                    DamagedItem item = x as DamagedItem;

                    if (item.Name != null)
                    {
                        if (item.Name.ToLower().Contains(FilterText.ToLower()))
                            return true;
                    }

                    if (item.Barcode != null)
                    {
                        if (item.Barcode.ToLower().Contains(FilterText.ToLower()))
                            return true;
                    }

                    if (item.Category != null)
                    {
                        if (item.Category.ToLower().Contains(FilterText.ToLower()))
                            return true;
                    }

                    if (item.Price.ToString().Contains(FilterText.ToLower()))
                        return true;


                    return false;
                });

                view.Refresh();

            }
        }

        private void ExecuteAddToCartCommand()
        {
            List<string> barcodes = new List<string>();

            foreach (var item in Items)
            {
                if (item.Selected)
                {
                    barcodes.Add(item.Barcode);
                }
            }
            aggregator.GetEvent<EventAggregation.MissingBarcodeEvent>().Publish(null);
            aggregator.GetEvent<EventAggregation.MissingBarcodeItemAddToCartCompleteEvent>().Publish(barcodes.ToArray());

            base.Dispose();       


        }

        BackgroundWorker worker = new BackgroundWorker();
        bool overylayVisible;

        public bool OverylayVisible
        {
            get { return overylayVisible; }
            set { overylayVisible = value; RaisePropertyChanged(() => OverylayVisible); }
        }

        bool cancelButtonEnabled;

        public bool CancelButtonEnabled
        {
            get { return cancelButtonEnabled; }
            set { cancelButtonEnabled = value; RaisePropertyChanged(() => CancelButtonEnabled); }
        }
        

        public override void Init()
        {
       
            //worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            //worker.DoWork += worker_DoWork;
            //worker.RunWorkerAsync();
            CancelButtonEnabled = false;
            aggregator.GetEvent<EventAggregation.OpenDamagedBarcodeItemsPanel>().Subscribe(ReloadItems);

            IsTabEnabled = true;
            TabSelectedIndex = 9;

            base.Init();


        }

        private void ReloadItems(object parameter = null)
        {
            FilterText = "";
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
        }


        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Items != null)
            {
                Items.Clear();
                Items = null;
            }
            //else {

                Items = repo.GetAllDamaged();

            //}

            
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OverylayVisible = false;
            CancelButtonEnabled = true;
        }

        private bool CanExecuteAddToCartCommand()
        {
            if (Items != null)
            {
                return Convert.ToBoolean(Items.Count(x => x.Selected == true));
            }
            else
            {
                return false;
            }
        }

        public override void Dispose()
        {
            worker.RunWorkerCompleted -= worker_RunWorkerCompleted;
            worker.DoWork -= worker_DoWork;
            
            base.Dispose();
        }
    }
}
