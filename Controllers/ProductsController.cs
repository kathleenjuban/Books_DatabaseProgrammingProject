using DBProg_A3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DBProg_A3.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        /// <summary>
        ///     The view to return a list of all products. Table columns are searchable and sortable (descending/ascending).
        ///     Search: Once user enters the item to be searched in the textbox, it will search for that item.
        ///     Sort: Once user clicks a table column, column is sorted. 
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <param name="sortBy">Integer 0 = ProductCode, 1 = Description, 2 = UnitPrice, 3 = OnHandQuantity</param>
        /// <param name="isDesc">The Sort Descending Boolean</param>
        /// <returns>AllProducts View</returns>
        public ActionResult AllProducts(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Product> products;

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                            products = context.Products.OrderByDescending(p => p.Description).ToList();
                        else
                            products = context.Products.OrderBy(p => p.Description).ToList();
                        break;
                    }
                case 2:
                    {
                        if (isDesc)
                            products = context.Products.OrderByDescending(p => p.UnitPrice).ToList();
                        else
                            products = context.Products.OrderBy(p => p.UnitPrice).ToList();
                        break;
                    }
                case 3:
                    {
                        if (isDesc)
                            products = context.Products.OrderByDescending(p => p.OnHandQuantity).ToList();
                        else
                            products = context.Products.OrderBy(p => p.OnHandQuantity).ToList();
                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                            products = context.Products.OrderByDescending(p => p.ProductCode).ToList();
                        else
                            products = context.Products.OrderBy(p => p.ProductCode).ToList();
                        break;
                    }
            }

            //id is used as searchTerm param
            //if id is not null or not whiteSpace, trim it
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                // Where is a LINQ. This is to filter search.
                products = products.Where(p =>
                                      p.ProductCode.ToLower().Contains(id) ||
                                      p.Description.ToLower().Contains(id) ||
                                      p.UnitPrice.ToString().Contains(id) ||
                                      p.OnHandQuantity.ToString().Contains(id)
                                      ).ToList();
            }

            products = products.Where(p => p.IsDeleted == false).ToList();
            return View(products);
        }

        // GET: Product
        /// <summary>
        ///     View to allow the user to upsert a product by retrieving the product first
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <returns>Product View</returns>
        [HttpGet]
        public ActionResult UpsertProduct(string id)
        {
            BooksEntities context = new BooksEntities();
            Product product = context.Products.Where(p => p.ProductCode == id).FirstOrDefault();

            if (product == null)
            {
                product = new Product();
            }

            return View(product);
        }

        // POST: Product
        /// <summary>
        ///     View to allow the user to post a new product
        /// </summary>
        /// <param name="newProduct">New product to be added</param>
        /// <returns>Redirection View to All Products</returns>

        [HttpPost]
        public ActionResult UpsertProduct(Product newProduct)
        {
            BooksEntities context = new BooksEntities();
            try
            {
                if (context.Products.Where(p => p.ProductCode == newProduct.ProductCode).Count() > 0)
                {
                    //Product already exists
                    var productToSave = context.Products.Where(p =>
                                        p.ProductCode == newProduct.ProductCode
                                        ).FirstOrDefault();

                    productToSave.Description = newProduct.Description;
                    productToSave.UnitPrice = newProduct.UnitPrice;
                    productToSave.OnHandQuantity = newProduct.OnHandQuantity;
                }
                else
                {
                    context.Products.Add(newProduct);
                }

                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("AllProducts");
        }

        // DELETE: Product
        /// <summary>
        ///     View to allow the user to delete a product.
        ///     Product is deleted on the front-end but the record is saved on the back-end (IsDeleted = true).
        /// </summary>
        /// <param name="id">The Search Term</param>
        /// <returns>JSON with three fields: Success Status, Id, and Products View</returns>

        [HttpGet]
        public ActionResult Delete(string id)
        {
            // Delete product with id from the database
            BooksEntities context = new BooksEntities();

            try
            {
                Product product = context.Products.Where(p => p.ProductCode == id).FirstOrDefault();
                product.IsDeleted = true;
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
            
            return Json(new
            {
                Success = true,
                Id = id,
                returnUrl = "/Products/AllProducts"
            }, JsonRequestBehavior.AllowGet);
        }

        // Top Five Products
        /// <summary>
        ///     Allows the user to view the top five highest-priced products
        /// </summary>
        /// <returns>Top Five Highest-Priced Products</returns>
        public ActionResult TopFiveProducts()
        {
            BooksEntities context = new BooksEntities();
            List<Product> products;
            products = context.Products.OrderByDescending(p => p.UnitPrice).Take(5).ToList();
            return View(products);
        }
    }
}
