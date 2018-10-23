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
    class MissingBarcodeViewModel : BaseViewModel
    {

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        
        readonly static MissingBarcodeCategoriesViewModel missingBarcodeCategories = new MissingBarcodeCategoriesViewModel();
        readonly static MissingBarcodeItemsViewModel missingBarcodeItems = new MissingBarcodeItemsViewModel();
        readonly static MissingBarcodeItemViewModel missingBarcodeItem = new MissingBarcodeItemViewModel();

        private BaseViewModel currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get
            {
                return currentViewModel;
            }
            set
            {
                if (currentViewModel == value)
                    return;

                currentViewModel = value;
                RaisePropertyChanged(() => CurrentViewModel);
            }
        }

        public override void Init()
        {
            base.Init();
        }

        public MissingBarcodeViewModel()
        {
            aggregator.GetEvent<EventAggregation.MissingBarcodeCategorySelectCompleteEvent>().Subscribe(ShowMissingItems);
            aggregator.GetEvent<EventAggregation.MissingBarcodeItemSelectCompleteEvent>().Subscribe(ShowMissingItem);
            //aggregator.GetEvent<EventAggregation.MissingBarcodeItemSelectCancelEvent>().Subscribe(ShowMissingCategories);

            ShowMissingCategories(null);
        }

        private void ShowMissingItem(object obj)
        {
            missingBarcodeItem.Init(Convert.ToInt32(obj));
            CurrentViewModel = MissingBarcodeViewModel.missingBarcodeItem;
        }

        private void ShowMissingCategories(object obj)
        {
            CurrentViewModel = MissingBarcodeViewModel.missingBarcodeCategories;
        }

        private void ShowMissingItems(object obj)
        {
            missingBarcodeItems.ItemSelector = "Category";
            missingBarcodeItems.Init(Convert.ToInt32(obj));
            CurrentViewModel = MissingBarcodeViewModel.missingBarcodeItems;
        }

        public override void Dispose()
        {
            CurrentViewModel = MissingBarcodeViewModel.missingBarcodeCategories;
        }
    }
}
