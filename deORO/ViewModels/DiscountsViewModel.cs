using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    class DiscountsViewModel : BaseViewModel
    {

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public ICommand DiscountSelectCommand { get { return new DelegateCommandWithParam(ExecuteDiscountSelectCommand); } }
        public ICommand PreviousPageCommand { get { return new DelegateCommand(ExecutePreviousPageCommand, CanExecutePreviousPageCommand); } }
        public ICommand NextPageCommand { get { return new DelegateCommand(ExecuteNextPageCommand, CanExecuteNextPageCommand); } }

        public ICommand CancelCommand { get { return new DelegateCommand(() => { aggregator.GetEvent<EventAggregation.DiscountSelectCancelEvent>().Publish(null); }); } }
        
        //IDialogService popup = new PopupViewService(800, 450);
        //IDialogService message = new MessageBoxViewService();

        private DiscountRepository repo = new DiscountRepository();
        private List<discount> discounts;

        private DispatcherTimer timer = new DispatcherTimer();

        public List<discount> Discounts
        {
            get
            {
                return discounts;
            }
            set
            {
                discounts = value;
                RaisePropertyChanged(() => Discounts);
            }
        }

        public override void Init()
        {
            Discounts = repo.GetDiscounts();

            timer.Interval = new TimeSpan(0, 1, 0);
            timer.Tick += timer_Tick;
            timer.Start();

            base.Init();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Minute == 0)
            {
                timer.IsEnabled = false;
                Discounts = repo.GetDiscounts();
                timer.IsEnabled = true;
            }
        }

        public DiscountsViewModel()
        {

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
            Discounts = repo.GetDiscounts(CurrentPage);
        }

        private void ExecuteNextPageCommand()
        {
            CurrentPage++;
            Discounts = repo.GetDiscounts(CurrentPage);
        }

        private bool CanExecuteNextPageCommand()
        {
            if (Discounts == null)
                return false;

            if (Discounts.Count() < 4)
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



        private void ExecuteDiscountSelectCommand(object obj)
        {
            int id = Convert.ToInt32(obj);
            if (repo.GetDiscount(DateTime.Now, id, 0) != null)
            {
                aggregator.GetEvent<EventAggregation.DiscountSelectCompleteEvent>().Publish(obj);
            }
            else
            {
                aggregator.GetEvent<EventAggregation.MissingBarcodeItemSelectCancelEvent>().Publish(null);
                DialogViewService.ShowAutoCloseDialog("Discount", "No items are qualified for this discount");
            }
        }

        public override void Dispose()
        {

        }
    }
}
