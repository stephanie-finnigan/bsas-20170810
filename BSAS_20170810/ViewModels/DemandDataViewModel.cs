using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSAS_20170810.Models;

namespace BSAS_20170810.ViewModels
{
    public class DemandDataViewModel
    {
        public IEnumerable<Demand> Demands { get; set; }

        public IEnumerable<DemandItem> DemandItems { get; set; }
    }
}