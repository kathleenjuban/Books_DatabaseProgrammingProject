using DBProg_A3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DBProg_A3.Controllers
{
    public class CustomersController : Controller
    {
        // GET: Customers
        /// <summary>
        ///     View to return a list of all customers. Table columns are searchable and sortable (descending/ascending).
        ///     Search: Once user enters the item to be searched in the textbox, it will search for that item.
        ///     Sort: Once user clicks a table column, column is sorted. 
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <param name="sortBy">Integer 0 = CustomerID, 1 = Name, 2 = Address, 3 = City, 4 = State, 5 = ZipCode</param>
        /// <param name="isDesc">The Sort Descending Boolean</param>
        /// <returns>AllCustomers View</returns>
        public ActionResult AllCustomers(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Customer> customers;

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                            customers = context.Customers.OrderByDescending(c => c.Name).ToList();
                        else
                            customers = context.Customers.OrderBy(c => c.Name).ToList();
                        break;
                    }
                case 2:
                    {
                        if (isDesc)
                            customers = context.Customers.OrderByDescending(c => c.Address).ToList();
                        else
                            customers = context.Customers.OrderBy(c => c.Address).ToList();
                        break;
                    }
                case 3:
                    {
                        if (isDesc)
                            customers = context.Customers.OrderByDescending(c => c.City).ToList();
                        else
                            customers = context.Customers.OrderBy(c => c.City).ToList();
                        break;
                    }
                case 4:
                    {
                        if (isDesc)
                            customers = context.Customers.OrderByDescending(c => c.State).ToList();
                        else
                            customers = context.Customers.OrderBy(c => c.State).ToList();
                        break;
                    }
                case 5:
                    {
                        if (isDesc)
                            customers = context.Customers.OrderByDescending(c => c.ZipCode).ToList();
                        else
                            customers = context.Customers.OrderBy(c => c.ZipCode).ToList();
                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                            customers = context.Customers.OrderByDescending(c => c.CustomerID).ToList();
                        else
                            customers = context.Customers.OrderBy(c => c.CustomerID).ToList();
                        break;
                    }
            }

            //id is used as searchTerm param
            //if id is not null or not whiteSpace, trim it
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                // Where is a LINQ. This is to filter search.
                customers = customers.Where(c =>
                                      c.CustomerID.ToString().Contains(id) ||
                                      c.Name.ToLower().Contains(id) ||
                                      c.Address.ToLower().Contains(id) ||
                                      c.City.ToLower().Contains(id) ||
                                      c.State.ToLower().Contains(id) ||
                                      c.ZipCode.ToLower().Contains(id)
                                      ).ToList();
            }

            //Show only items which are not deleted
            customers = customers.Where(c => c.IsDeleted == false).ToList();

            return View(customers);
        }

        // GET: Customer
        /// <summary>
        ///     View to allow the user to upsert a customer by retrieving customer first
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <returns>UpsertCustomer View</returns>
        [HttpGet]
        public ActionResult UpsertCustomer(int id = 0) // Set 0 as default instead of manually adding to a.href
        {

            BooksEntities context = new BooksEntities();
            Customer customer = context.Customers.Where(c => c.CustomerID == id).FirstOrDefault();
            List<State> states = context.States.ToList();

            if (customer == null)
            {
                customer = new Customer();
            }

            UpsertCustomerModel viewModel = new UpsertCustomerModel()
            {
                Customer = customer,
                States = states
            };

            return View(viewModel);
        }

        // POST: Customer
        /// <summary>
        ///     View to allow the user to post a new customer
        /// </summary>
        /// <param name="model">UpsertCustomerModel</param>
        /// <param name="StateName">String Statename</param>
        /// <param name="CustomerAddress">String CustomerAddress</param>
        /// <param name="CustomerCity">String CustomerCity</param>
        /// <param name="CustomerZipCode">String Customer ZipCode</param>
        /// <returns>Redirection View to All Customers</returns>
        [HttpPost]
        public ActionResult UpsertCustomer(UpsertCustomerModel model, string StateName, string CustomerAddress, string CustomerCity, string CustomerZipCode)
        {
            Customer newCustomer = model.Customer;

            StateName = StateName.Split('(')[1].Replace(")", "");
            newCustomer.State = StateName;
            newCustomer.Address = CustomerAddress;
            newCustomer.City = CustomerCity;
            newCustomer.ZipCode = CustomerZipCode;

            BooksEntities context = new BooksEntities();
            try
            {
                if (context.Customers.Where(c => c.CustomerID == newCustomer.CustomerID).Count() > 0)
                {
                    //Customer already exists
                    var customerToSave = context.Customers.Where(c =>
                                        c.CustomerID == newCustomer.CustomerID
                                        ).FirstOrDefault();

                    customerToSave.Name = newCustomer.Name;
                    customerToSave.Address = newCustomer.Address;
                    customerToSave.City = newCustomer.City;
                    customerToSave.State = newCustomer.State;
                    customerToSave.ZipCode = newCustomer.ZipCode;
                }
                else
                {
                    context.Customers.Add(newCustomer);
                }

                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("AllCustomers");
        }
        
        // DELETE: Customer
        /// <summary>
        ///     View to allow the user to delete a customer.
        ///     Customer is deleted on the front-end but his record is saved on the back-end (IsDeleted = true).
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <returns>JSON with three fields: Success Status, Id, and Customers View</returns>

        [HttpGet]
        public ActionResult Delete(string id)
        {
            // Delete customer with id from the database
            BooksEntities context = new BooksEntities();
            int customerId = 0;
            if (int.TryParse(id, out customerId))
            {
                try
                {
                    Customer customer = context.Customers.Where(c => c.CustomerID == customerId).FirstOrDefault();
                    // Once customer is successfully deleted, IsDeleted property changes to true
                    customer.IsDeleted = true;
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    //throw;
                    //return ex.Message;
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
                returnUrl = "/Customers/AllCustomers"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}