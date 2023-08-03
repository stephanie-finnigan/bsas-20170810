using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSAS_20170810.Models;

namespace BSAS_20170810.ViewModels
{
    public class DemandItemViewModel
    {
        public Demand Demand { get; set; }

        public DemandItem Item { get; set; }

        public int ItemQuantityId { get; set; }

        public IEnumerable<Holding> Holdings { get; set; }
    }
}