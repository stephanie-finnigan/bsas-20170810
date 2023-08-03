using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BSAS_20170810.Models
{
    public class Invoice
    {
        [Key]
        public int RecordId { get; set; }
        public string InvoiceId { get; set; }
        public int HoldingId { get; set; }
        public int ItemQuantity { get; set; }
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }
        public virtual Holding Holding { get; set; }
    }
}