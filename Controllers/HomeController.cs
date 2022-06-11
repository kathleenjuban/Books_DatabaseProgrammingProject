using DBProg_A3.Models;
using Square;
using Square.Exceptions;
using Square.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DBProg_A3.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // STATISTICS
        /// <summary>
        ///     Allows the user to view database statistics (ie top five highest priced products, top five sales by invoice)
        /// </summary>
        /// <returns></returns>
        public ActionResult Statistics()
        {
            ViewBag.Message = "Book App Statistics";

            return View();
        }

        // Square Payments
        /// <summary>
        ///     Allows the user to create a Square payment
        /// </summary>
        /// <returns></returns>
        public ActionResult Payments()
        {
            ViewBag.Message = "Square Payments";

            SquareClient client = new SquareClient.Builder()
                                    .Environment(Square.Environment.Sandbox)
                                    .AccessToken("EAAAEDZL52oSCH_t7jiYRuU9RoeFHm-Ds_apis4Y9Ifx3AqcKcwrGdeRU5JujNil")
                                    .Build();

            // create a payment from Square
            var amountMoney = new Money.Builder()
                              .Amount(950L)
                              .Currency("USD")
                              .Build();

            var body = new CreatePaymentRequest.Builder(
                sourceId: "cnon:card-nonce-ok",
                idempotencyKey: Guid.NewGuid().ToString(),
                amountMoney: amountMoney)
                .Build();

            try
            {
                var result = client.PaymentsApi.CreatePayment(body: body);
            }
            catch (ApiException e)
            {
                Console.WriteLine("Failed to make the request");
                Console.WriteLine($"Response Code: {e.ResponseCode}");
                Console.WriteLine($"Exception: {e.Message}");
            }

            return View();
        }
    }
}