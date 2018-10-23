using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    class ItemViewModel : BaseViewModel
    {
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }
        public ICommand SaveCommand { get { return new DelegateCommand(ExecuteSaveCommand); } }

        private item item;

        private ItemRepository repo = new ItemRepository();
        ManufacturerRepository manuReop = new ManufacturerRepository();
        CategoryRepository catRepo = new CategoryRepository();
        DiscountRepository discountRepo = new DiscountRepository();

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public item Item
        {
            get { return item; }
            set
            {
                item = value;
                RaisePropertyChanged(() => Item);
            }
        }

        private List<category> categories;

        public List<category> Categories
        {
            get
            {
                categories = catRepo.GetCategories();
                return categories;
            }
            set
            {
                categories = value;
                RaisePropertyChanged(() => Categories);
            }
        }
        private List<manufacturer> manufacturers;

        public List<manufacturer> Manufacturers
        {
            get
            {
                manufacturers = manuReop.GetManufacturers();
                return manufacturers;
            }
            set
            {
                manufacturers = value;
                RaisePropertyChanged(() => Manufacturers);
            }
        }
        private List<discount> discounts;

        public List<discount> Discounts
        {
            get
            {
                discounts = discountRepo.GetDiscounts();
                return discounts;
            }
            set
            {
                discounts = value;
                RaisePropertyChanged(() => Discounts);
            }
        }

        public ItemViewModel()
        {
            item = new item();
        }

        public ItemViewModel(int itemId)
        {
            item = new item();
            item = repo.GetItem(itemId);
        }

        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }

        private void ExecuteSaveCommand()
        {
            //if (item.id == 0)
            //{
            //    if (repo.AddItem(item))
            //    {
            //        aggregator.GetEvent<EventAggregation.ItemAddCompleteEvent>().Publish(null);
            //    }
            //    else
            //    {
            //        aggregator.GetEvent<EventAggregation.ItemAddFailEvent>().Publish(null);
            //    }
            //}
            //else
            //{
            //    if (repo.UpdateItem(item))
            //    {
            //        aggregator.GetEvent<EventAggregation.ItemUpdateCompleteEvent>().Publish(null);
            //    }
            //    else
            //    {
            //        aggregator.GetEvent<EventAggregation.ItemUpdateFailEvent>().Publish(null);
            //    }
            //}

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }
    }
}
