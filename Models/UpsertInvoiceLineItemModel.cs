using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBProg_A3.Models
{
    public class UpsertInvoiceLineItemModel
    {
        public InvoiceLineItem InvoiceLineItem { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<Product> Products { get; set; }
    }
}