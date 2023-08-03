using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BSAS_20170810.Models
{
    public class HoldingCategory
    {
        public int Id { get; set; }

        [Display (Name = "Category")]
        public string CategoryName { get; set; }

        public List<Holding> Holdings { get; set; }
    }
}