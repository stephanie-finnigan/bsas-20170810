using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSAS_20170810.Models;
using BSAS_20170810.ViewModels;
using Microsoft.AspNet.Identity;

namespace BSAS_20170810.Controllers
{
    public class DemandInvoiceController : Controller
    {
        BSASDbContext db = new BSASDbContext();
        // GET: DemandInvoice
        public ActionResult Index()
        {
            var invoice = DemandInvoice.GetInvoice(this.HttpContext);

            var viewModel = new DemandItemViewModel
            {
                InvoiceItems = invoice.GetInvoiceItems()
            };

            return View(viewModel);
        }

        public ActionResult AddToDemand(int id, List<Holding> items)
        {
            var addedHolding = db.Holdings.Single(h => h.HoldingId == id);
            //foreach (var addedHolding in items)
            //{
            //    addedHolding = db.Holdings.Single(h => h.HoldingId == id);
            //}

            var invoice = DemandInvoice.GetInvoice(this.HttpContext);

            invoice.AddToDemandInvoice(addedHolding);
            return RedirectToAction("Create", "Demands");
        }

        [HttpPost]
        public ActionResult RemoveFromInvoice(int id)
        {
            var invoice = DemandInvoice.GetInvoice(this.HttpContext);

            string holdingName = db.Invoices.Single(i => i.RecordId == id).Holding.HoldingName;

            int itemCount = invoice.RemoveFromDemandInvoice(id);

            var results = new DemandInvoiceRemoveViewModel
            {
                Message = Server.HtmlEncode(holdingName) + "has been removed from your invoice.",
                InvoiceCount = invoice.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }

        [ChildActionOnly]
        public ActionResult InvoiceSummary()
        {
            var invoice = DemandInvoice.GetInvoice(this.HttpContext);

            ViewData["InvoiceCount"] = invoice.GetCount();
            return PartialView("CartSummary");
        }
    }
}