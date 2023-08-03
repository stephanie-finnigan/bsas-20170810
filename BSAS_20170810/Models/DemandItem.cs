using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BSAS_20170810.Models
{
    public class DemandItem
    {
        public int DemandItemId { get; set; }

        public int DemandId { get; set; }
        public Demand Demand { get; set; }

        public int HoldingId { get; set; }
        public Holding Holding { get; set; }

        public int ItemQuantity { get; set; }

        public DemandItem() { }

        public DemandItem(Holding holding, Demand demand, int itemQuantity)
        {
            this.Holding = holding;
            this.Demand = demand;
            this.ItemQuantity = itemQuantity;
        }
    }
}