using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using deORO.Helpers;
using System.Windows.Controls;
using System.Timers;
using Microsoft.Practices.Composite.Events;
using System.Collections.Generic;
using System.Linq;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using System.Windows.Threading;
using deORO.EventAggregation;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using deORODataAccessApp.Models;
using System.Reflection;
using System.Windows.Forms;


namespace deORO.ViewModels
{
    public class HardwareViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public ICommand CoinHopperResetCommand { get { return new DelegateCommand(ExecuteCoinHopperResetCommand); } }
        public ICommand BillRecyclerResetCommand { get { return new DelegateCommand(ExecuteBillRecyclerResetCommand); } }
        public ICommand MDBCoinResetCommand { get { return new DelegateCommand(ExecuteMDBCoinResetCommand); } }
        public ICommand MDBBillResetCommand { get { return new DelegateCommand(ExecuteMDBBillResetCommand); } }



        private deORO.Communication.ICommunicationType commType;

        public override void Init()
        {

            //if (Helpers.Global.CoinHopper == true)
            //{
            ResetBillRecyclerVisable = true;
            ResetCoinHopperEnable = true;
            ResetMDBBillEnable = true;
            ResetMDBCoinEnable = true;
            //}
            //else if (Helpers.Global.BillDispenser == true)
            //{
            //    ResetBillRecyclerVisable = true;
            //    ResetCoinHopperEnable = false;
            //    ResetMDBBillEnable = true;
            //    ResetMDBCoinEnable = false;
            //}

            commType = Communication.CommunicationTypeFactory.GetCommunicationType();
           
            base.Init();         


        }

        private void ExecuteBillRecyclerResetCommand()
        {
            commType.ResetBillRecycler();
        }

        private void ExecuteCoinHopperResetCommand()
        {
            commType.ResetCoinHopper();
        }

        private void ExecuteMDBCoinResetCommand()
        {
            commType.ResetMDBCoin();
        }

        private void ExecuteMDBBillResetCommand()
        {
            commType.ResetMDBBill();
        }



        private bool resetBillRecyclerVisable = false;
        public bool ResetBillRecyclerVisable
        {
            get { return resetBillRecyclerVisable; }
            set { resetBillRecyclerVisable = value; RaisePropertyChanged(() => ResetBillRecyclerVisable); }
        }

        private bool resetCoinHopperEnable = false;
        public bool ResetCoinHopperEnable
        {
            get { return resetCoinHopperEnable; }
            set { resetCoinHopperEnable = value; RaisePropertyChanged(() => ResetCoinHopperEnable); }
        }

        private bool resetMDBBillEnable = false;
        public bool ResetMDBBillEnable
        {
            get { return resetMDBBillEnable; }
            set { resetMDBBillEnable = value; RaisePropertyChanged(() => ResetMDBBillEnable); }
        }

        private bool resetMDBCoinEnable = false;
        public bool ResetMDBCoinEnable
        {
            get { return resetMDBCoinEnable; }
            set { resetMDBCoinEnable = value; RaisePropertyChanged(() => ResetMDBCoinEnable); }
        }

    }
}
