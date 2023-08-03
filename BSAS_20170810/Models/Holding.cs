using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSAS_20170810.Models
{
    public class Holding
    {
        public int HoldingId { get; set; }

        [Display (Name = "NSN")]
        public string NSN { get; set; }

        [Display (Name = "Name")]
        public string HoldingName { get; set; }

        public int CategoryId { get; set; }
        public HoldingCategory Category { get; set; }
        [NotMapped]
        public string CategoryName { get; set; }

        [Display (Name = "Description")]
        public string Description { get; set; }

        [Display (Name = "Quantity In Stock")]
        public int QuantityInStock { get; set; }

        public HoldingCondition Condition { get; set; }

        [Display (Name = "External Part Number")]
        public string PartNumber { get; set; }
    }
}