using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.Models;

namespace deORO.DataAccess
{
    public class ShoppingCartRepository
    {
        deOROEntities entities = new deOROEntities();

        public string SaveShoppingCart(List<ShoppingCartItem> shoppingItems, List<PaymentItem> paymentItems, TransactionError tranError, string userPkId)
        {
            try
            {
                DateTime shoppingDateTime = DateTime.Now;
                shoppingcart cart = new shoppingcart();
                cart.pkid = Guid.NewGuid().ToString();

                if (userPkId != "")
                {
                    cart.userpkid = userPkId;
                }

                cart.created_date_time = shoppingDateTime;
                entities.shoppingcarts.Add(cart);

                foreach (PaymentItem payItem in paymentItems)
                {
                    payment pay = new payment();
                    pay.pkid = Guid.NewGuid().ToString();
                    pay.shoppingcartpkid = cart.pkid;
                    pay.source = payItem.Source;
                    pay.amount = payItem.Payment;
                    pay.routing = payItem.Routing;
                    pay.created_date_time = payItem.DateTime;

                    entities.payments.Add(pay);
                }

                foreach (ShoppingCartItem item in shoppingItems)
                {
                    shoppingcartdetail sDetail = new shoppingcartdetail();
                    sDetail.pkid = Guid.NewGuid().ToString();
                    sDetail.shoppingcartpkid = cart.pkid;
                    sDetail.itemid = item.Id;
                    sDetail.barcode = item.BarCode;
                    sDetail.created_date_time = DateTime.Now;
                    sDetail.price_tax_included = item.PriceTaxIncluded;
                    sDetail.price = item.Price;
                    sDetail.tax = item.Tax;

                    sDetail.discount_percentage = item.DiscountPercentage;
                    sDetail.discount_description = item.DiscountDescription;
                    sDetail.discount_price = item.DiscountPrice;
                    sDetail.discount_tax = item.DiscountTax;
                    sDetail.original_price = item.OriginalPrice;
                    sDetail.original_tax = item.OriginalTax;
                    sDetail.tax = item.Tax;
                    sDetail.crv = item.Crv;
                    
                    entities.shoppingcartdetails.Add(sDetail);
                }

                if (tranError != null)
                {
                    transactionerror error = new transactionerror();
                    error.pkid = Guid.NewGuid().ToString();
                    error.shoppingcartpkid = cart.pkid;
                    error.source = tranError.Source;
                    error.amount = tranError.Amount;
                    error.created_date_time = tranError.DateTime;
                    error.@event = tranError.Event;
                    entities.transactionerrors.Add(error);

                }

                entities.SaveChanges();

                ItemRepository itemRepo = new ItemRepository();
                itemRepo.UpdateItemsQuantity(shoppingItems);

                return cart.pkid;
            }
            catch
            {
                return "";
            }
        }

        public List<shoppingcart>GetList(DateTime? lastSync = null)
        {
            return entities.shoppingcarts.Where(x => x.created_date_time >= lastSync).ToList();
        }
    }
}
