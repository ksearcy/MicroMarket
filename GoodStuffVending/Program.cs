using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace GoodStuffVending
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Enter the location name you want to Convert");
            string locationName = Console.ReadLine();

            LocationRepository sourceLocationRepo = new LocationRepository();
            InventoryRepository sourceInventoryRepo = new InventoryRepository();
            UserRepository sourceUserRepo = new UserRepository();
            TransactionHistoryRepository souceTransactionRepo = new TransactionHistoryRepository();
            StockRepository sourceStockRepo = new StockRepository();

            deORODataAccess.ItemRepository destinationItemRepo = new deORODataAccess.ItemRepository();
            deORODataAccess.CategoryRepository destinationCategoryRepo = new deORODataAccess.CategoryRepository();
            deORODataAccess.LocationRepository destinationLocationRepo = new deORODataAccess.LocationRepository();
            deORODataAccess.ShoppingCartRepository destinationShoppingCartRepo = new deORODataAccess.ShoppingCartRepository();
            deORODataAccess.ShoppingCartDetailRepository destinationShoppingCartDetailRepo = new deORODataAccess.ShoppingCartDetailRepository();
            deORODataAccess.PaymentRepository destinationPaymentRepo = new deORODataAccess.PaymentRepository();
            deORODataAccess.UserRepository destinationUserRepo = new deORODataAccess.UserRepository();
            deORODataAccess.LocationItemRepository destinationLocItemRepo = new deORODataAccess.LocationItemRepository();


            var sourceLocation = sourceLocationRepo.FindBy(x => x.location1 == locationName).FirstOrDefault();
            var destLocation = destinationLocationRepo.FindBy(x => x.name == locationName).FirstOrDefault();

            if (destLocation != null && sourceLocation != null)
            {
                #region Users

                //var users = sourceUserRepo.GetAll();

                //foreach (var sUser in users)
                //{
                //    deORODataAccess.user dUser = new deORODataAccess.user();
                //    dUser.pkid = Guid.NewGuid().ToString();
                //    dUser.username = sUser.userName;
                //    dUser.password = sUser.password;
                //    dUser.created_date_time = sUser.createdDate;
                //    dUser.last_updated_on = sUser.modifiedDate;
                //    dUser.is_active = 1;
                //    dUser.customerid = destLocation.customerid;
                //    dUser.locationid = destLocation.id;
                //    dUser.account_balance = Convert.ToDecimal(sUser.accountBalance);

                //    if (sUser.role.ToString().Equals("9ca849fc-dd2c-45af-aa9a-76ac6046ec7b"))
                //    {
                //        dUser.is_superuser = 0;
                //        dUser.is_staff = 0;
                //    }
                //    else
                //    {
                //        dUser.is_superuser = 1;
                //        dUser.is_staff = 1;
                //    }

                //    dUser.is_approved = 1;
                //    destinationUserRepo.Add(dUser);
                //}

                //destinationUserRepo.Save();
                //Console.WriteLine("Converted {0} Users", users.Count);
                #endregion

                #region Categories & Inventories

                //var categories = sourceInventoryRepo.GetCategories();

                //foreach (var c in categories)
                //{
                //    if (c != null && !c.Equals(""))
                //    {
                //        deORODataAccess.category ca = new deORODataAccess.category();
                //        ca.name = c;
                //        ca.code = c;
                //        ca.description = c;
                //        ca.parentid = 1;
                //        ca.pick_order = 0;
                //        destinationCategoryRepo.Add(ca);
                //    }
                //}

                //destinationCategoryRepo.Save();
                //Console.WriteLine("Converted {0} Sub Category Items", categories.Count());

                //var inventories = sourceInventoryRepo.GetAll();
                //foreach (var i in inventories)
                //{
                //    deORODataAccess.item item = new deORODataAccess.item();
                //    item.name = i.name;
                //    item.barcode = i.barcode;
                //    item.upc = i.barcode;
                //    item.price = Convert.ToDecimal(i.price);
                //    item.tax = Convert.ToDecimal(i.tax);
                //    item.crv = Convert.ToDecimal(i.crv);
                //    item.description = i.description;
                //    item.categoryid = 1;
                //    try
                //    {
                //        item.subcategoryid = destinationCategoryRepo.FindBy(x => x.code == i.category).FirstOrDefault().id;
                //    }
                //    catch { }
                //    item.price_tax_included = (item.price ?? 0) + (item.tax ?? 0) + (item.crv ?? 0);
                //    item.is_active = 1;
                //    item.is_taxable = i.tax == null ? (byte)0 : (byte)1;

                //    destinationItemRepo.Add(item);
                //}

                //destinationItemRepo.Save();

                //var items = destinationItemRepo.GetAll().Select(x => x.id);
                //int[] itemIds = items.ToArray();

                //destinationLocItemRepo.AddRemoveItems(destLocation.customerid.Value, destLocation.id, itemIds);

                var stocks = sourceStockRepo.FindBy(x=>x.kioskID == sourceLocation.kioskID);

                foreach (var stock in stocks)
                {
                   
                    string barcode = sourceInventoryRepo.FindBy(x=>x.inventoryID == stock.inventoryID).FirstOrDefault().barcode;
                    int itemID = 0;

                    if (destinationItemRepo.FindBy(x => x.barcode == barcode).FirstOrDefault() != null)
                    {
                        itemID = destinationItemRepo.FindBy(x => x.barcode == barcode).FirstOrDefault().id;
                    }
                    else
                    {
                        var i = sourceInventoryRepo.GetSingleById(x => x.barcode == barcode);
                        
                        deORODataAccess.item item = new deORODataAccess.item();
                        item.name = i.name;
                        item.barcode = i.barcode;
                        item.upc = i.barcode;
                        item.price = Convert.ToDecimal(i.price);
                        item.tax = Convert.ToDecimal(i.tax);
                        item.crv = Convert.ToDecimal(i.crv);
                        item.description = i.description;
                        item.categoryid = 1;
                        try
                        {
                            item.subcategoryid = destinationCategoryRepo.FindBy(x => x.code == i.category).FirstOrDefault().id;
                        }
                        catch { }
                        item.price_tax_included = (item.price ?? 0) + (item.tax ?? 0) + (item.crv ?? 0);
                        item.is_active = 1;
                        item.is_taxable = i.tax == null ? (byte)0 : (byte)1;

                        destinationItemRepo.Add(item);
                        destinationItemRepo.Save();

                        itemID = destinationItemRepo.FindBy(x => x.barcode == barcode).FirstOrDefault().id;
                    }

                    //int itemID = destinationItemRepo.FindBy(x => x.barcode == barcode).FirstOrDefault().id;

                    deORODataAccess.location_item li = destinationLocItemRepo.FindBy(x => x.itemid == itemID && x.locationid == destLocation.id).SingleOrDefault();

                    if (li == null)
                    {
                        li = new deORODataAccess.location_item();

                        li.customerid = destLocation.customerid.Value;
                        li.locationid = destLocation.id;
                        li.itemid = itemID;
                        li.is_taxable = stock.tax == null ? (byte)0 : (byte)1;
                        li.price = stock.price;
                        li.crv = stock.crv;
                        li.tax_percent = stock.tax * 100;
                        li.tax = ((li.price == null ? 0 : li.price) * (li.tax_percent == null ? 0 : li.tax_percent)) / 100;
                        //li.price_tax_included = (li.price == null ? 0 : li.price) + (li.crv == null ? 0 : li.crv) + (li.tax == null ? 0 : li.tax);
                        li.price_tax_included = (li.price == null ? 0 : li.price) + (li.crv == null ? 0 : li.crv) + (li.tax == null ? 0 : li.tax);

                        destinationLocItemRepo.Add(li);

                    }
                    else
                    {
                        li.customerid = destLocation.customerid.Value;
                        li.locationid = destLocation.id;
                        li.itemid = itemID;
                        li.is_taxable = stock.tax == null ? (byte)0 : (byte)1;
                        li.price = stock.price;
                        li.crv = stock.crv;
                        li.tax_percent = stock.tax * 100;
                        li.tax = ((li.price == null ? 0 : li.price) * (li.tax_percent == null ? 0 : li.tax_percent)) / 100;
                        //li.price_tax_included = (li.price == null ? 0 : li.price) + (li.crv == null ? 0 : li.crv) + (li.tax == null ? 0 : li.tax);
                        li.price_tax_included = (li.price == null ? 0 : li.price) + (li.crv == null ? 0 : li.crv) + (li.tax == null ? 0 : li.tax);

                        destinationLocItemRepo.Edit(li);
                    }
                   
                }

                destinationLocItemRepo.Save();

                //Console.WriteLine("Converted {0} Inventory Items", inventories.Count());
                #endregion
                
                //#region TransactionHistory

                //var transactions = souceTransactionRepo.FindBy(x => x.kioskID == sourceLocation.kioskID);

                //int j = 0;
                //foreach (var tran in transactions)
                //{
                //    Console.WriteLine("Processing {0}", j++);

                //    deORODataAccess.shoppingcart cart = new deORODataAccess.shoppingcart();
                //    cart.pkid = Guid.NewGuid().ToString();
                //    cart.locationid = destLocation.id;
                //    cart.customerid = destLocation.customerid;

                //    try
                //    {
                //        cart.userpkid = destinationUserRepo.FindBy(x => x.username == tran.userID).FirstOrDefault().pkid;
                //    }
                //    catch { }
                //    cart.created_date_time = tran.dateOfPurchase;
                //    destinationShoppingCartRepo.Add(cart);
                //    //destinationShoppingCartRepo.Save();

                //    XElement element = XElement.Parse(tran.what);

                //    foreach (var node in element.Elements("item"))
                //    {
                //        deORODataAccess.shoppingcartdetail detail = new deORODataAccess.shoppingcartdetail();
                //        detail.pkid = Guid.NewGuid().ToString();
                //        detail.locationid = destLocation.id;
                //        detail.customerid = destLocation.customerid;
                //        detail.itemid = destinationItemRepo.FindBy(x => x.barcode == node.Value.Trim()).FirstOrDefault().id;
                //        detail.barcode = node.Value.Trim();
                //        detail.shoppingcartpkid = cart.pkid;
                //        detail.tax = tran.tax;
                //        detail.crv = tran.crv;
                //        detail.price_tax_included = tran.total;
                //        detail.price = tran.total - (tran.crv + tran.tax);
                //        detail.created_date_time = tran.dateOfPurchase;

                //        destinationShoppingCartDetailRepo.Add(detail);
                //    }

                //    //destinationShoppingCartDetailRepo.Save();

                //    deORODataAccess.payment pay = new deORODataAccess.payment();
                //    pay.pkid = Guid.NewGuid().ToString();
                //    pay.shoppingcartpkid = cart.pkid;
                //    pay.customerid = destLocation.customerid;
                //    pay.locationid = destLocation.id;


                //    if (tran.cash.Value)
                //    {
                //        pay.source = "BillPay";
                //    }
                //    else if (tran.credit.Value)
                //    {
                //        pay.source = "CreditCardPay";
                //    }
                //    else if (tran.debit.Value)
                //    {
                //        pay.source = "DebitCardPay";
                //    }

                //    pay.amount = tran.total;
                //    pay.created_date_time = tran.dateOfPurchase;
                //    destinationPaymentRepo.Add(pay);
                //    //destinationPaymentRepo.Save();

                //}

                //destinationShoppingCartRepo.Save();
                //destinationShoppingCartDetailRepo.Save();
                //destinationPaymentRepo.Save();

                //Console.WriteLine("Converted {0} Transactions", transactions.Count());
                //#endregion
            }
            else
            {
                Console.WriteLine("Location does not exist");
                Console.ReadLine();
            }

            Console.ReadLine();
        
        }
    }
}
