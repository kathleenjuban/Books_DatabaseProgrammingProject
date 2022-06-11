using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBProg_A3.Models
{
    public class UpsertInvoiceModel
    {
        public Invoice Invoice { get; set; }
        public List<Customer> Customers { get; set; }
    }
}