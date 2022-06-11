using DBProg_A3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DBProg_A3.Controllers
{
    public class InvoiceLineItemsController : Controller
    {
        // GET: InvoiceLineItems
        /// <summary>
        ///     View to return a list of all invoice line items. Table columns are searchable and sortable (descending/ascending).
        ///     Search: Once user enters the item to be searched in the textbox, it will search for that item.
        ///     Sort: Once user clicks a table column, column is sorted. 
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <param name="sortBy">Integer 0 = InvoiceID, 1 = ProductCode, 2 = UnitPrice, 3 = Quantity, 4 = ItemTotal, 5 = InvoiceId</param>
        /// <param name="isDesc">The Sort Descending Boolean</param>
        /// <returns>AllInvoiceLineItems View</returns>
        public ActionResult AllInvoiceLineItems(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<InvoiceLineItem> invoiceLineItems;

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                            invoiceLineItems = context.InvoiceLineItems.OrderByDescending(ii => ii.ProductCode).ToList();
                        else
                            invoiceLineItems = context.InvoiceLineItems.OrderBy(ii => ii.ProductCode).ToList();
                        break;
                    }
                case 2:
                    {
                        if (isDesc)
                            invoiceLineItems = context.InvoiceLineItems.OrderByDescending(ii => ii.UnitPrice).ToList();
                        else
                            invoiceLineItems = context.InvoiceLineItems.OrderBy(ii => ii.UnitPrice).ToList();
                        break;
                    }
                case 3:
                    {
                        if (isDesc)
                            invoiceLineItems = context.InvoiceLineItems.OrderByDescending(ii => ii.Quantity).ToList();
                        else
                            invoiceLineItems = context.InvoiceLineItems.OrderBy(ii => ii.Quantity).ToList();
                        break;
                    }
                case 4:
                    {
                        if (isDesc)
                            invoiceLineItems = context.InvoiceLineItems.OrderByDescending(ii => ii.ItemTotal).ToList();
                        else
                            invoiceLineItems = context.InvoiceLineItems.OrderBy(ii => ii.ItemTotal).ToList();
                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                            invoiceLineItems = context.InvoiceLineItems.OrderByDescending(ii => ii.InvoiceID).ToList();
                        else
                            invoiceLineItems = context.InvoiceLineItems.OrderBy(ii => ii.InvoiceID).ToList();
                        break;
                    }
            }

            //id is used as searchTerm param
            //if id is not null or not whiteSpace, trim it
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();
                int idLookup = 0;

                if (int.TryParse(id, out idLookup))
                {
                    invoiceLineItems = invoiceLineItems.Where(ii =>
                                                              ii.InvoiceID == idLookup
                                                              ).ToList();
                }
                else
                {
                    // Where is a LINQ. This is to filter search.
                    invoiceLineItems = invoiceLineItems.Where(ii =>
                                          ii.ProductCode.ToLower().Contains(id) ||
                                          ii.UnitPrice.ToString().Contains(id) ||
                                          ii.Quantity.ToString().Contains(id) ||
                                          ii.ItemTotal.ToString().Contains(id)
                                          ).ToList();
                }
            }

            //Show only items which are not deleted
            invoiceLineItems = invoiceLineItems.Where(ii => ii.IsDeleted == false).ToList();

            return View(invoiceLineItems);
        }

        // GET: InvoiceLineItem
        /// <summary>
        ///     View to allow the user to upsert an invoice line item by retrieving the line item first
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <returns>UpsertInvoiceLineItemModel View</returns>
        [HttpGet]
        public ActionResult UpsertInvoiceLineItem(int invoiceId, string productCode) // Manually add ?invoiceId=0 to a.href to add an invoice line item
        {

            BooksEntities context = new BooksEntities();
            
            InvoiceLineItem invoiceLineItem = context.InvoiceLineItems
                                                .Where(ii => ii.InvoiceID == invoiceId && ii.ProductCode == productCode)
                                                .FirstOrDefault();
            List<Invoice> invoices = context.Invoices.ToList();
            List<Product> products = context.Products.ToList();

            if (invoiceLineItem == null)
            {
                invoiceLineItem = new InvoiceLineItem();
            }

            UpsertInvoiceLineItemModel viewModel = new UpsertInvoiceLineItemModel()
            {
                InvoiceLineItem = invoiceLineItem,
                Invoices = invoices,
                Products = products
            };

            return View(viewModel);
        }

        // POST: InvoiceLineItem
        /// <summary>
        ///      View to allow the user to post a new invoice line item
        /// </summary>
        /// <param name="model">UpsertInvoiceLineItemModel</param>
        /// <param name="InvoiceID">Integer InvoiceId</param>
        /// <param name="ProductName">String ProductName</param>
        /// <returns>Redirection View to All Invoice Line Items</returns>
        [HttpPost]

        public ActionResult UpsertInvoiceLineItem(UpsertInvoiceLineItemModel model, int InvoiceID, string ProductName)
        {
            InvoiceLineItem newInvoiceLineItem = model.InvoiceLineItem;
            newInvoiceLineItem.InvoiceID = InvoiceID;
            newInvoiceLineItem.ProductCode = ProductName;

            BooksEntities context = new BooksEntities();
            try
            {
                if (context.InvoiceLineItems.Where(ii => ii.InvoiceID == newInvoiceLineItem.InvoiceID && 
                                                         ii.ProductCode == newInvoiceLineItem.ProductCode
                                                  ).Count() > 0)
                {
                    //InvoiceLineItem already exists
                    var invoiceLineItemToSave = context.InvoiceLineItems.Where(ii =>
                                        ii.InvoiceID == newInvoiceLineItem.InvoiceID &&
                                        ii.ProductCode == newInvoiceLineItem.ProductCode
                                        ).FirstOrDefault();

                    invoiceLineItemToSave.UnitPrice = newInvoiceLineItem.UnitPrice;
                    invoiceLineItemToSave.Quantity = newInvoiceLineItem.Quantity;
                    invoiceLineItemToSave.ItemTotal = newInvoiceLineItem.ItemTotal;
                }
                else
                {
                    context.InvoiceLineItems.Add(newInvoiceLineItem);
                }

                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("AllInvoiceLineItems");
        }

        // GET: InvoiceLineItem
        /// <summary>
        ///     View to allow the user to delete an invoice line item.
        ///     Invoice line item is deleted on the front-end but the record is saved on the back-end (IsDeleted = true).
        /// </summary>
        /// <param name="invoiceId">Integer InvoiceId</param>
        /// <param name="productCode">String ProductCode</param>
        /// <returns>JSON with four fields: Success Status, Id, ProdCode, and InvoiceLineItems View</returns>
        [HttpGet]
        public ActionResult Delete(int invoiceId, string productCode)
        {
            // Delete invoice line item with matching invoiceId and productCode from the database
            BooksEntities context = new BooksEntities();           

            try
            {
                InvoiceLineItem invoiceLineItem = context.InvoiceLineItems.Where(ii => ii.InvoiceID == invoiceId &&
                                                                                ii.ProductCode == productCode
                                                                                ).FirstOrDefault();
                invoiceLineItem.IsDeleted = true;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                
                return Json(new
                {
                    Success = false,
                    Id = invoiceId,
                    ProdCode = productCode,
                    Message = ex.Message
                });
            }

            return Json(new
            {
                Success = true,
                Id = invoiceId,
                ProdCode = productCode,
                returnUrl = "/InvoiceLineItems/AllInvoiceLineItems"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
