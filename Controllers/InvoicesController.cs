using DBProg_A3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DBProg_A3.Controllers
{
    public class InvoicesController : Controller
    {
        // GET: Invoices
        /// <summary>
        ///     The view to return a list of all invoices. Table columns are searchable and sortable (descending/ascending).
        ///     Search: Once user enters the item to be searched in the textbox, it will search for that item.
        ///     Sort: Once user clicks a table column, column is sorted. 
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <param name="sortBy">Integer 0 = InvoiceID, 1 = CustomerID, 2 = InvoiceDate, 3 = ProductTotal, 4 = SalesTax, 5 = Shipping, 6 = InvoiceTotal</param>
        /// <param name="isDesc">The Sort Descending Boolean</param>
        /// <returns>AllInvoices View</returns>
        public ActionResult AllInvoices(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Invoice> invoices;

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                            invoices = context.Invoices.OrderByDescending(i => i.Customer.Name).ToList();
                        else
                            invoices = context.Invoices.OrderBy(i => i.Customer.Name).ToList();
                        break;
                    }
                case 2:
                    {
                        if (isDesc)
                            invoices = context.Invoices.OrderByDescending(i => i.InvoiceDate).ToList();
                        else
                            invoices = context.Invoices.OrderBy(i => i.InvoiceDate).ToList();
                        break;
                    }
                case 3:
                    {
                        if (isDesc)
                            invoices = context.Invoices.OrderByDescending(i => i.ProductTotal).ToList();
                        else
                            invoices = context.Invoices.OrderBy(i => i.ProductTotal).ToList();
                        break;
                    }
                case 4:
                    {
                        if (isDesc)
                            invoices = context.Invoices.OrderByDescending(i => i.SalesTax).ToList();
                        else
                            invoices = context.Invoices.OrderBy(i => i.SalesTax).ToList();
                        break;
                    }
                case 5:
                    {
                        if (isDesc)
                            invoices = context.Invoices.OrderByDescending(i => i.Shipping).ToList();
                        else
                            invoices = context.Invoices.OrderBy(i => i.Shipping).ToList();
                        break;
                    }
                case 6:
                    {
                        if (isDesc)
                            invoices = context.Invoices.OrderByDescending(i => i.InvoiceTotal).ToList();
                        else
                            invoices = context.Invoices.OrderBy(i => i.InvoiceTotal).ToList();
                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                            invoices = context.Invoices.OrderByDescending(i => i.InvoiceID).ToList();
                        else
                            invoices = context.Invoices.OrderBy(i => i.InvoiceID).ToList();
                        break;
                    }
            }

            //id is used as searchTerm param
            //if id is not null or not whiteSpace, trim it
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                // Where is a LINQ. This is to filter search.
                invoices = invoices.Where(i =>
                                      i.InvoiceID.ToString().Contains(id) ||
                                      i.Customer.Name.ToLower().Contains(id) ||
                                      i.InvoiceDate.ToString().Contains(id) ||
                                      i.ProductTotal.ToString().Contains(id) ||
                                      i.SalesTax.ToString().Contains(id) ||
                                      i.Shipping.ToString().Contains(id) ||
                                      i.InvoiceTotal.ToString().Contains(id)
                                      ).ToList();
            }

            invoices = invoices.Where(i => i.IsDeleted == false).ToList();
            return View(invoices);
        }

        // GET: Invoice
        /// <summary>
        ///     View to allow the user to upsert an invoice by retrieving the invoice first
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <returns>UpsertInvoiceModel</returns>
        [HttpGet]
        public ActionResult UpsertInvoice(int id = 0) // Set 0 as default instead of manually adding to a.href
        {

            BooksEntities context = new BooksEntities();
            Invoice invoice = context.Invoices.Where(i => i.InvoiceID == id).FirstOrDefault();
            List<Customer> customers = context.Customers.ToList();

            if (invoice == null)
            {
                invoice = new Invoice();
            }

            UpsertInvoiceModel viewModel = new UpsertInvoiceModel()
            {
                Invoice = invoice,
                Customers = customers
            };

            return View(viewModel);
        }

        // POST: Invoice
        /// <summary>
        ///      View to allow the user to post a new invoice 
        /// </summary>
        /// <param name="model">UpsertInvoiceModel</param>
        /// <param name="CustomerName">String CustomerName</param>
        /// <returns>Redirection View to All Invoices</returns>

        [HttpPost]

        public ActionResult UpsertInvoice(UpsertInvoiceModel model, string CustomerName)
        {
            Invoice newInvoice = model.Invoice;

            CustomerName = CustomerName.Split('(')[1].Replace(")", "");
            newInvoice.CustomerID = Convert.ToInt32(CustomerName);

            BooksEntities context = new BooksEntities();
            try
            {
                if (context.Invoices.Where(i => i.InvoiceID == newInvoice.InvoiceID).Count() > 0)
                {
                    //Invoice already exists
                    var invoiceToSave = context.Invoices.Where(i =>
                                        i.InvoiceID == newInvoice.InvoiceID
                                        ).FirstOrDefault();

                    invoiceToSave.CustomerID = newInvoice.CustomerID;
                    invoiceToSave.InvoiceDate = newInvoice.InvoiceDate;
                    invoiceToSave.ProductTotal = newInvoice.ProductTotal;
                    invoiceToSave.SalesTax = newInvoice.SalesTax;
                    invoiceToSave.Shipping = newInvoice.Shipping;
                    invoiceToSave.InvoiceTotal = newInvoice.InvoiceTotal;
                }
                else
                {
                    context.Invoices.Add(newInvoice);
                }

                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("AllInvoices");
        }

        // GET: Delete
        /// <summary>
        ///     View to allow the user to delete an invoice.
        ///     Invoice is deleted on the front-end but the record is saved on the back-end (IsDeleted = true).
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <returns>JSON with three fields: Success Status, Id, and Invoices View</returns>

        [HttpGet]
        public ActionResult Delete(string id)
        {
            // Delete invoice with id from the database
            BooksEntities context = new BooksEntities();
            int invoiceId = 0;
            if (int.TryParse(id, out invoiceId))
            {
                try
                {
                    Invoice invoice = context.Invoices.Where(i => i.InvoiceID == invoiceId).FirstOrDefault();
                    invoice.IsDeleted = true;
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Success = false,
                        Id = id,
                        Message = ex.Message
                    });
                }
            }
            else
            {
                //unsuccessful parse
            }

            return Json(new
            {
                Success = true,
                Id = id,
                returnUrl = "/Invoices/AllInvoices"
            }, JsonRequestBehavior.AllowGet);
        }

        // Top Five Invoices Based on Invoice Amount
        /// <summary>
        ///     View to allow user to check the current top five invoices based on invoice amount
        /// </summary>
        /// <returns>Top Five Invoices Based on Invoice Amount</returns>
        public ActionResult TopFiveInvoices()
        {
            BooksEntities context = new BooksEntities();
            List<Invoice> invoices;
            invoices = context.Invoices.OrderByDescending(i => i.InvoiceTotal).Take(5).ToList();
            return View(invoices);
        }
    }
}
