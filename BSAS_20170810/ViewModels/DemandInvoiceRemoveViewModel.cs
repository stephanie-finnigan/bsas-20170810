using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSAS_20170810.ViewModels
{
    public class DemandInvoiceRemoveViewModel
    {
        public string Message { get; set; }

        public int InvoiceCount { get; set; }

        public int ItemCount { get; set; }

        public int DeleteId { get; set; }
    }
}