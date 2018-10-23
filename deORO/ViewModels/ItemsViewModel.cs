using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;
using deORODataAccessApp.Models;
using deORO.BarcodeScanner;
using deORO.EventAggregation;

namespace deORO.ViewModels
{
    class ItemsViewModel : BaseViewModel
    {
        ItemRepository repo = new ItemRepository();
        TransactionErrorRepository transactionErrorRepositiry = new TransactionErrorRepository();
        //IDialogService popup = new PopupViewService(800, 850);
        //IDialogService message = new MessageBoxViewService();

        public ICommand SaveItemCommand { get { return new DelegateCommandWithParam(ExecuteSaveItemCommand); } }
        public ICommand AllItemsCommand { get { return new DelegateCommand(ProcessAllItemsCommand); } }
        public ICommand ServicedItemsCommand { get { return new DelegateCommand(ProcessServicedItemsCommand); } }

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        private IBarcodeScanner barcodeScanner = BarcodeScanner.BarcodeScannerFactory.GetBarcodeScanner();

        private List<Item> items;
        private string barcode;

        public string Barcode
        {
            get { return barcode; }
            set
            {
                barcode = value;
                RaisePropertyChanged(() => Barcode);
                ApplyFilter();
            }
        }

        private bool allItemsChecked = true;

        public bool AllItemsChecked
        {
            get { return allItemsChecked; }
            set
            {
                allItemsChecked = value;
                RaisePropertyChanged(() => AllItemsChecked);
            }
        }
        private bool serviceItemsChecked;

        public bool ServiceItemsChecked
        {
            get { return serviceItemsChecked; }
            set
            {
                serviceItemsChecked = value;
                RaisePropertyChanged(() => ServiceItemsChecked);
            }
        }

        public override void Init()
        {
            barcodeScanner.Open("Local");
            aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Subscribe(ProcessBarcode);

            base.Init();
        }

        private void ProcessAllItemsCommand()
        {
            AllItemsChecked = true;
            ServiceItemsChecked = false;
        }

        private void ProcessServicedItemsCommand()
        {
            ServiceItemsChecked = true;
            AllItemsChecked = false;
        }

        private void ProcessBarcode(object obj)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    Barcode = obj.ToString();
                }
                catch { }
            });
        }

        private void ApplyFilter()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(items);

            if (view != null)
            {
                view.Filter = ((x) =>
                    {
                        if (barcode == "" || barcode == null) return true;

                        Item item = x as Item;

                        if (item.name != null)
                        {
                            if (item.name.ToLower().Contains(barcode.ToLower()))
                                return true;
                        }

                        if (item.barcode != null)
                        {
                            if (item.barcode.Contains(barcode))
                                return true;
                        }

                        return false;
                    });

                view.Refresh();

            }
        }

        public List<Item> Items
        {
            get
            {
                items = GetItemsList();
                return items;
            }
            set
            {
                items = value;
                RaisePropertyChanged(() => Items);
            }
        }

        private List<Item> GetItemsList()
        {
            if (ServiceItemsChecked)
            {
                return repo.GetServicedList();
            }
            else
            {
                return repo.GetList();
            }
        }

        private void ExecuteSaveItemCommand(object obj)
        {
            Item item = obj as Item;
            Item original = Items.Where(x => x.id == item.id).SingleOrDefault();

            if (original.Quantity != item.Quantity || item.Stale > 0)
            {
                item.Short += (item.Quantity + (item.Stale.HasValue ? item.Stale.Value : 0)) - original.Quantity;

                if (item.Short != 0)
                {

                    transaction_error transactionError = new transaction_error();

                    transactionError.description = "InventoryAdjust";
                    transactionError.amount = Convert.ToDecimal(item.Short);
                    transactionError.source = Helpers.Enum.DeviceType.InventoryAdjusted.ToString();
                    transactionError.code = item.barcode;
                    transactionError.pkid = Guid.NewGuid().ToString();

                    transactionErrorRepositiry.AddItem(transactionError);

                    //==================INVENTORY ADJUSTMENT DYNAMIC NOTIFICATION===================

                    Global.DynamicPanelDialogTitle = "Inventory Adjusted";
                    Global.DynamicPanelDialogMessage = "Inventory Adjusted";
                    Global.DynamicPanelDialogType = "Succsessful";

                    aggregator.GetEvent<EventAggregation.ShowDynamicPanelDialog>().Publish(null);

                    //==================METHOD COMMENTED BECAUSE WAS OPENING A POPUP FOR EACH INVENTORY ADJUSTMENT===================

                    //aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                    // {
                    //     Event = "InventoryAdjust",
                    //     DeviceType = Helpers.Enum.DeviceType.InventoryAdjusted,
                    //     Amount = Convert.ToDecimal(item.Short),
                    //     Code = item.barcode
                    // });
                }

                if (item.Stale != 0)
                {

                    transaction_error transactionError = new transaction_error();

                    transactionError.description = "InventoryStale";
                    transactionError.amount = Convert.ToDecimal(item.Stale);
                    transactionError.source = Helpers.Enum.DeviceType.InventoryAdjusted.ToString();
                    transactionError.code = item.barcode;
                    transactionError.pkid = Guid.NewGuid().ToString();

                    transactionErrorRepositiry.AddItem(transactionError);

                    //==================METHOD COMMENTED BECAUSE WAS OPENING A POPUP FOR EACH INVENTORY ADJUSTMENT===================

                    //aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                    //{
                    //    Event = "InventoryStale",
                    //    DeviceType = Helpers.Enum.DeviceType.InventoryAdjusted,
                    //    Amount = Convert.ToDecimal(item.Stale),
                    //    Code = item.barcode
                    //});
                }

            }

            if (!repo.UpdateItem(item))
            {
                DialogViewService.ShowAutoCloseDialog("Save Item", "No changes detected from last save.");
            }
            else
            {
                Items = GetItemsList();
                ApplyFilter();
            }
        }

        public override void Dispose()
        {
            barcodeScanner.Close();
            aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Unsubscribe(ProcessBarcode);
        }
    }
}
