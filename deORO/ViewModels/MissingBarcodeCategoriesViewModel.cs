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
    class MissingBarcodeCategoriesViewModel : BaseViewModel
    {
        CategoryRepository repo = new CategoryRepository();

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand CategorySelectCommand { get { return new DelegateCommandWithParam(ExecuteCategorySelectCommand); } }
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }
        public ICommand PreviousPageCommand { get { return new DelegateCommand(ExecutePreviousPageCommand, CanExecutePreviousPageCommand); } }
        public ICommand NextPageCommand { get { return new DelegateCommand(ExecuteNextPageCommand, CanExecuteNextPageCommand); } }

        private List<category> categories;
        public List<category> Categories
        {
            get
            {
                return categories;
            }
            set
            {
                categories = value;
                RaisePropertyChanged(() => Categories);
            }
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
            Categories = repo.GetMissingItemsSubCategories(CurrentPage);
        }

        private void ExecuteNextPageCommand()
        {
            CurrentPage++;
            Categories = repo.GetMissingItemsSubCategories(CurrentPage);
        }

        private bool CanExecuteNextPageCommand()
        {
            if (Categories == null)
                return false;

            if (Categories.Count() < 8)
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

        public override void Init()
        {
            Categories = repo.GetMissingItemsSubCategories();
            base.Init();
        }


        private void ExecuteCategorySelectCommand(object obj)
        {
            aggregator.GetEvent<EventAggregation.MissingBarcodeCategorySelectCompleteEvent>().Publish(obj);
        }

        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.MissingBarcodeCategorySelectCancelEvent>().Publish(null);
        }

    }
}
