using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BSAS_20170810.Models
{
    public class DemandStatus
    {
        public int Id { get; set; }

        [Display (Name = "Status:")]
        public string StatusName { get; set; }

        public List<Demand> Demands { get; set; }
    }
}