using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BSAS_20170810.Models
{
    public class Station
    {
        public int StationId { get; set; }

        [Display (Name = "Station")]
        public string StationName { get; set; }

        public int RegionId { get; set; }
        public StationRegion Region { get; set; }

        [NotMapped]
        public string RegionName { get; set; }

        public List<User> Users { get; set; }
    }
}