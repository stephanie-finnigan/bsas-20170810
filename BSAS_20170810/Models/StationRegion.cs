using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BSAS_20170810.Models
{
    public class StationRegion
    {
        public int Id { get; set; }

        [Display (Name = "Region")]
        public string RegionName { get; set; }

        public List<Station> Stations { get; set; }
    }
}