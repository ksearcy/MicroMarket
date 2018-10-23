using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Composite.Events;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using System.Windows.Input;
using deORO.Helpers;

namespace deORO.ViewModels
{
    public class MissingBarcodeItemViewModel : BaseViewModel
    {
        ItemRepository repo = new ItemRepository();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public ICommand AddToCartCommand { get { return new DelegateCommand(ExecuteAddToCartCommand, CanExecuteAddToCartCommand); } }
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }
        public ICommand UpCommand { get { return new DelegateCommand(ExecuteUpCommand); } }
        public ICommand DownCommand { get { return new DelegateCommand(ExecuteDownCommand); } }


        private void ExecuteDownCommand()
        {
            if (Quantity != 0)
                Quantity--;
        }

        private bool CanExecuteAddToCartCommand()
        {
            return Convert.ToBoolean(Quantity);
        }

        private void ExecuteUpCommand()
        {
            Quantity++;
        }

        private void ExecuteAddToCartCommand()
        {
            string[] barcodes = new string[quantity];

            for (int i = 0; i < quantity; i++)
            {
                barcodes[i] = _item.barcode;
            }

            aggregator.GetEvent<EventAggregation.MissingBarcodeItemAddToCartCompleteEvent>().Publish(barcodes);
        }

        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.MissingBarcodeItemAddToCartCancelEvent>().Publish(null);
        }

        private decimal totalPrice = 0.0m;
        public decimal TotalPrice
        {
            get
            {
                return totalPrice;
            }
            set
            {
                totalPrice = value;
                RaisePropertyChanged(() => TotalPrice);
            }
        }

        private int quantity = 0;
        public int Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                quantity = value;
                RaisePropertyChanged(() => Quantity);

                if (_item != null)
                    TotalPrice = _item.price.Value * Quantity;
            }
        }


        private item _item;
        public item Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                RaisePropertyChanged(() => Item);
            }
        }

        public void Init(int itemId)
        {
            Item = repo.GetItem(itemId);
        }

        public override void Dispose()
        {
            Quantity = 0;
            TotalPrice = 0.0m;
            Item = null;
        }

    }
}
