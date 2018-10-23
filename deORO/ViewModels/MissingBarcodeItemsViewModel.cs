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
    public class MissingBarcodeItemsViewModel : BaseViewModel
    {
        ItemRepository repo = new ItemRepository();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }
        public ICommand ItemSelectCommand { get { return new DelegateCommandWithParam(ExecuteItemSelectCommand); } }
        public ICommand PreviousPageCommand { get { return new DelegateCommand(ExecutePreviousPageCommand, CanExecutePreviousPageCommand); } }
        public ICommand NextPageCommand { get { return new DelegateCommand(ExecuteNextPageCommand, CanExecuteNextPageCommand); } }

        private int id;
        private string itemSelector;

        public string ItemSelector
        {
            get { return itemSelector; }
            set { itemSelector = value; }
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

        private void ExecutePreviousPageCommand()
        {
            CurrentPage--;

            if (itemSelector == "Category")
            {
                Items = repo.GetItemsByCategory(id, CurrentPage);
            }
            else if (itemSelector == "Discount")
            {
                Items = repo.GetItemsByDiscount(id, CurrentPage);
            }
        }

        private void ExecuteNextPageCommand()
        {
            CurrentPage++;

            if (itemSelector == "Category")
            {
                Items = repo.GetItemsByCategory(id, CurrentPage);
            }
            else if (itemSelector == "Discount")
            {
                Items = repo.GetItemsByDiscount(id, CurrentPage);
            }
        }

        private bool CanExecuteNextPageCommand()
        {
            if (Items.Count() < 8)
                return false;
            else
                return true;
        }

        private bool CanExecutePreviousPageCommand()
        {
            if (currentPage == 1)
                return false;
            else
                return true;
        }

        private void ExecuteItemSelectCommand(object obj)
        {
            aggregator.GetEvent<EventAggregation.MissingBarcodeItemSelectCompleteEvent>().Publish(obj);

        }

        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.MissingBarcodeItemSelectCancelEvent>().Publish(null);
        }

        private List<item> items;
        public List<item> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                RaisePropertyChanged(() => Items);
            }
        }


        public MissingBarcodeItemsViewModel()
        {

        }

        public void Init(int id)
        {
            this.id = id;

            if (itemSelector == "Category")
            {
                Items = repo.GetItemsByCategory(id);
            }
            else if (itemSelector == "Discount")
            {
                Items = repo.GetItemsByDiscount(id);
            }
        }
    }
}
