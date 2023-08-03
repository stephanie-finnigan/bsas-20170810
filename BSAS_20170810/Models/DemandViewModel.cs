using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSAS_20170426.Models
{
    public class DemandViewModel
    {
        public Demand demand { get; set; }
        public ICollection<Holding> holdings { get; set; }
        public ICollection<DemandItem> demandItems { get; set; }
    }
}