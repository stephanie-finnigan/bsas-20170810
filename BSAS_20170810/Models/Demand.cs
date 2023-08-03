using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace BSAS_20170810.Models
{
    public class Demand
    {
        // Next
        public int DemandId { get; set; }
        //Next
        [Display(Name = "Submitted On:")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "dd-MMM-yyyy")]
        public DateTime SubmitDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        [NotMapped]
        public string StationName { get; set; }

        [Display(Name = "Special Instructions")]
        [DataType(DataType.MultilineText)]
        public string SpecialInstructions { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        public int StatusId { get; set; }
        public DemandStatus Status { get; set; }
        [NotMapped]
        public string StatusName { get; set; }

        [Display(Name = "Despatched on:")]
        public DateTime? DateDespatched { get; set; }

        [Display(Name = "Delivered on:")]
        public DateTime? DateDelivered { get; set; }

        [Display(Name = "Demand Items:")]
        public ICollection<DemandItem> DemandItems { get; set; }
    }
}