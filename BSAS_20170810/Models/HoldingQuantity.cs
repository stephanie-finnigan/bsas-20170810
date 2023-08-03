using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BSAS_20170810.Models
{
    public class HoldingQuantity
    {
        [Key]
        public int QuantityId { get; set; }
        public int QuantityInStock { get; set; }

        public int HoldingId { get; set; }
        public Holding Holding { get; set; }

        public int DemandId { get; set; }
        public Demand Demand { get; set; }

        //public int DemandItemId { get; set; }
        //public virtual DemandItem DemandItem { get; set; }

        //public int SupplierId { get; set; }
    }
}