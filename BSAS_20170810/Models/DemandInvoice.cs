using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSAS_20170810.Models
{
    public partial class DemandInvoice
    {
        private BSASDbContext db = new BSASDbContext();
        string DemandInvoiceId { get; set; }
        public const string InvoiceSessionKey = "InvoiceId";
        public static DemandInvoice GetInvoice(HttpContextBase context)
        {
            var invoice = new DemandInvoice();
            invoice.DemandInvoiceId = invoice.GetInvoiceId(context);
            return invoice;
        }

        public static DemandInvoice GetInvoice(Controller controller)
        {
            return GetInvoice(controller.HttpContext);
        }

        public void AddToDemandInvoice(Holding holding)
        {
            var invoiceItem = db.Invoices.SingleOrDefault(
                i => i.InvoiceId == DemandInvoiceId
                && i.HoldingId == holding.HoldingId);

            if (invoiceItem == null)
            {
                invoiceItem = new Invoice
                {
                    HoldingId = holding.HoldingId,
                    InvoiceId = DemandInvoiceId,
                    //ItemQuantity = quantity,
                    DateCreated = DateTime.Now
                };
                db.Invoices.Add(invoiceItem);
            }
            db.SaveChanges();
        }

        public int RemoveFromDemandInvoice(int id)
        {
            var invoiceItem = db.Invoices.Single(
                i => i.InvoiceId == DemandInvoiceId
                && i.RecordId == id);

            int itemCount = 0;

            if (invoiceItem != null)
            {
                if (invoiceItem.Count > 1)
                {
                    invoiceItem.Count--;
                    itemCount = invoiceItem.Count;
                }
                else
                {
                    db.Invoices.Remove(invoiceItem);
                }
                db.SaveChanges();
            }
            return itemCount;
        }

        public void EmptyInvoice()
        {
            var invoiceItems = db.Invoices.Where(
                i => i.InvoiceId == DemandInvoiceId);

            foreach (var item in invoiceItems)
            {
                db.Invoices.Remove(item);
            }
            db.SaveChanges();
        }
        public List<Invoice> GetInvoiceItems()
        {
            return db.Invoices.Where(
                i => i.InvoiceId == DemandInvoiceId).ToList();
        }
        public int GetCount()
        {
            int? count = (from invoiceItems in db.Invoices
                          where invoiceItems.InvoiceId == DemandInvoiceId
                          select (int?)invoiceItems.Count).Sum();
            return count ?? 0;
        }

        public int CreateDemand(Demand demand)
        {
            var invoiceItems = GetInvoiceItems();

            foreach (var item in invoiceItems)
            {
                var demandItem = new DemandItem
                {
                    HoldingId = item.HoldingId,
                    DemandId = demand.DemandId,
                    ItemQuantity = item.Count
                };

                db.DemandItems.Add(demandItem);
            }
            db.SaveChanges();
            EmptyInvoice();
            return demand.DemandId;
        }

        public string GetInvoiceId(HttpContextBase context)
        {
            if (context.Session[InvoiceSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[InvoiceSessionKey] = context.User.Identity.Name;
                }
            }
            return context.Session[InvoiceSessionKey].ToString();
        }
    }
}