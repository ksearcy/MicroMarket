using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using deORODataAccess;

namespace deOROService
{
    [ServiceContract]
    public interface ISyncDataService
    {
        [OperationContract]
        void UploadData(int customerId, int locationId, DataSet data, bool usersSharedAcrossLocations = false);

        [OperationContract]
        DataSet DownloadData(int customerId, int locationId, bool usersSharedAcrossLocations = false);

        [OperationContract]
        DataSet GetScheduleAndItemsQuantityInfo(int customerId, int locationId);

        [OperationContract]
        void UpdateScheduledStatus(int customerId, int locationId, DataSet ds);

        [OperationContract]
        bool TestService(int customerId, int locationId);

        [OperationContract]
        int AdjustUnevenBalances(int customerId, int locationId, bool usersSharedAcrossLocations = false);
    }

    public partial class SyncDataService
    {
        public DataSet DownloadData(int customerId, int locationId, bool usersSharedAcrossLocations = false)
        {
            System.Threading.Thread.Sleep(2000);
            ItemRepository repoItem = new ItemRepository(customerId, locationId);
            DiscountRepository repoDiscount = new DiscountRepository();
            CategoryRepository repoCategory = new CategoryRepository();
            PlanogramRepository planogram = new PlanogramRepository(customerId, locationId);
            ComboDiscountRepository repoComboDiscount = new ComboDiscountRepository(customerId, locationId);
            ComboDiscountDetailRepository repoComboDiscountDetail =
                new ComboDiscountDetailRepository(customerId, locationId);
            LocationCreditRepository repolocationCredit = new LocationCreditRepository(customerId, locationId);
            LocationCreditUserRepository repolocationCreditUser =
                new LocationCreditUserRepository(customerId, locationId);
            SubsidyRepository repoSubsidy = new SubsidyRepository(customerId, locationId);
            SubsidyDetailRepository repoSubsidyDetail = new SubsidyDetailRepository(customerId, locationId);


            DataSet ds = new DataSet();
            ds.Tables.Add(repoItem.GetAllByCustomerLocation().ToDataTable("Item"));
            ds.Tables.Add(repoDiscount.GetAll().ToList().ToDataTable("Discount"));
            ds.Tables.Add(repoCategory.GetAll().ToDataTable("Category"));
            ds.Tables.Add(planogram.GetAllByCustomerLocation().ToDataTable("PlanogramItem"));
            ds.Tables.Add(repoComboDiscount.GetAllByCustomerLocation().ToDataTable("ComboDiscount"));
            ds.Tables.Add(repoComboDiscountDetail.GetAllByCustomerLocation().ToDataTable("ComboDiscountDetail"));
            ds.Tables.Add(repolocationCredit.GetAllByCustomerLocation().ToDataTable("Credit"));
            ds.Tables.Add(repolocationCreditUser.GetAllByCustomerLocation().ToDataTable("CreditUser"));
            ds.Tables.Add(repoSubsidy.GetAllByCustomerLocation().ToDataTable("Subsidy"));
            ds.Tables.Add(repoSubsidyDetail.GetAllByCustomerLocation().ToDataTable("SubsidyDetail"));

            if (usersSharedAcrossLocations)
            {
                UserRepository userRepo = new UserRepository(customerId, locationId);
                ds.Tables.Add(userRepo.GetAllWhereLocationIsNull().ToDataTable("User"));
            }
            else {
                UserRepository userRepo = new UserRepository(customerId, locationId);
                ds.Tables.Add(userRepo.GetAll(customerid: customerId, locationid: locationId).ToDataTable("User"));
            }

            return ds;
        }

        
    }
}
    