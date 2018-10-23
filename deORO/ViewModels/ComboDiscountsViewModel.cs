using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using deORODataAccessApp.DataAccess;
using deORODataAccessApp.Models;

namespace deORO.ViewModels
{
    class ComboDiscountsViewModel : BaseViewModel
    {
        ComboDiscountRepository repo = new ComboDiscountRepository();

        List<ComboDiscount> discounts;

        public ICommand PreviousPageCommand { get { return new DelegateCommand(ExecutePreviousPageCommand, CanExecutePreviousPageCommand); } }
        public ICommand NextPageCommand { get { return new DelegateCommand(ExecuteNextPageCommand, CanExecuteNextPageCommand); } }

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

        private int count = 0;

        private void ExecutePreviousPageCommand()
        {
            CurrentPage--;
            Discounts = repo.GetActiveDiscounts(CurrentPage);
        }

        private void ExecuteNextPageCommand()
        {
            CurrentPage++;
            Discounts = repo.GetActiveDiscounts(CurrentPage);
        }

        private bool CanExecuteNextPageCommand()
        {
            if (Discounts == null)
                return false;

            if (currentPage == count)
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

        public List<ComboDiscount> Discounts
        {
            get { return discounts; }
            set { discounts = value; RaisePropertyChanged(() => Discounts); }
        }

        private bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; RaisePropertyChanged(() => IsVisible); }
        }

        public override void Init()
        {
            count = repo.GetActiveDiscountsCount();
            IsVisible = Convert.ToBoolean(count);

            Discounts = repo.GetActiveDiscounts();
            base.Init();
        }
    }
}
