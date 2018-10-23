using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp.Helpers;

namespace deORODataAccessApp.Models
{
    public class ShoppingCartItem : NotificationObject
    {

        private string guid;

        public string Guid
        {
            get { return guid; }
            set { guid = value; }
        }


        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private int discountId;

        public int DiscountId
        {
            get { return discountId; }
            set { discountId = value; }
        }

        private string barCode;

        public string BarCode
        {
            get { return barCode; }
            set
            {
                barCode = value;
                RaisePropertyChanged(() => BarCode);
            }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged(() => Name);
            }
        }
        private decimal price;

        public decimal Price
        {
            get { return price; }
            set
            {
                price = value;
                RaisePropertyChanged(() => Price);
            }
        }
        private decimal tax;

        public decimal Tax
        {
            get { return tax; }
            set
            {
                tax = value;
                RaisePropertyChanged(() => Tax);
            }
        }
        private decimal priceTaxIncluded;

        public decimal PriceTaxIncluded
        {
            get { return priceTaxIncluded; }
            set
            {
                priceTaxIncluded = value;
                RaisePropertyChanged(() => PriceTaxIncluded);
            }
        }
        private decimal discountPrice;

        public decimal DiscountPrice
        {
            get { return discountPrice; }
            set
            {
                discountPrice = value;
                RaisePropertyChanged(() => DiscountPrice);
            }
        }

        private decimal taxPercent;
        public decimal TaxPercent
        {
            get { return taxPercent; }
            set
            {
                taxPercent = value;
                RaisePropertyChanged(() => TaxPercent);
            }
        }

        private decimal discountTax;

        public decimal DiscountTax
        {
            get { return discountTax; }
            set
            {
                discountTax = value;
                RaisePropertyChanged(() => DiscountTax);
            }
        }

        private decimal crv;

        public decimal Crv
        {
            get { return crv; }
            set { crv = value; RaisePropertyChanged(() => Crv); }
        }

        private decimal discountPercentage;

        public decimal DiscountPercentage
        {
            get { return discountPercentage; }
            set { discountPercentage = value; }
        }
        private string discountDescription;

        public string DiscountDescription
        {
            get { return discountDescription; }
            set { discountDescription = value; }
        }
        private decimal originalPrice;

        public decimal OriginalPrice
        {
            get { return originalPrice; }
            set { originalPrice = value; }
        }

        private decimal originalTax;

        public decimal OriginalTax
        {
            get { return originalTax; }
            set { originalTax = value; }
        }

        private int combodiscountid;

        public int Combodiscountid
        {
            get { return combodiscountid; }
            set { combodiscountid = value; }
        }

        private bool combodiscountApplied;

        public bool CombodiscountApplied
        {
            get { return combodiscountApplied; }
            set { combodiscountApplied = value; RaisePropertyChanged(() => CombodiscountApplied); }
        }

        private int categoryid;

        public int Categoryid
        {
            get { return categoryid; }
            set { categoryid = value; }
        }

        private int subsidyId;

        public int SubsidyId
        {
            get { return subsidyId; }
            set { subsidyId = value; }
        }

        private string subsidyDescription;

        public string SubsidyDescription
        {
            get { return subsidyDescription; }
            set { subsidyDescription = value; }
        }
        private decimal subsidyPrice;

        public decimal SubsidyPrice
        {
            get { return subsidyPrice; }
            set { subsidyPrice = value; }
        }
        private decimal subsidyTax;

        public decimal SubsidyTax
        {
            get { return subsidyTax; }
            set { subsidyTax = value; }
        }
        private decimal subsidyPercentage;

        public decimal SubsidyPercentage
        {
            get { return subsidyPercentage; }
            set { subsidyPercentage = value; }
        }
    }
}
