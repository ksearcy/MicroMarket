using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.EventAggregation;
using deORO.MDB;
using Microsoft.Practices.Composite.Events;

namespace deORO.CashStatus
{
    public class CashStatus
    {
        private CashStatusRepository repo = new CashStatusRepository();
        
        public void GetCashStatus(string @event)
        {
            try
            {
                deORO.Communication.ICommunicationType commType = deORO.Communication.CommunicationTypeFactory.GetCommunicationType();
                CoinAndBillStatusEventArgs args = commType.GetCoinAndBillStatus();

                CashStatusRepository repo = new CashStatusRepository();
                string group = Guid.NewGuid().ToString();
                DateTime createdDateTime = DateTime.Now;

                cash_status status = new cash_status();
                status.pkid = Guid.NewGuid().ToString();
                status.group = group;
                status.description = "BillAcceptor";
                status.is_full = Convert.ToByte(args.Stacker.IsFull);
                status.count = args.Stacker.BillCount;
                status.created_date_time = createdDateTime;
                status.@event = @event;
                repo.AddCashStatus(status);

                foreach (TubeInfo c in args.Tubes)
                {
                    status = new cash_status();
                    status.pkid = Guid.NewGuid().ToString();
                    status.group = group;
                    status.description = "Tube " + c.Number;
                    status.count = c.CoinCount;
                    status.is_full = Convert.ToByte(c.IsFull);
                    status.created_date_time = createdDateTime;
                    status.@event = @event;
                    status.amount = c.Amount;
                    repo.AddCashStatus(status);
                }
            }
            catch
            {

            }
        }

    }
}
