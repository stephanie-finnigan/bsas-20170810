using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BSAS_20170810.Models;
using BSAS_20170810.ViewModels;
using System.Data.Entity.Infrastructure;
using System.Web.UI;

namespace BSAS_20170810.Controllers
{
    //[Authorize]
    public class DemandsController : Controller
    {
        private BSASDbContext db = new BSASDbContext();

        // GET: Demands
        public ActionResult Index(int? id)
        {
            var model = new DemandDataViewModel()
            {
                Demands = db.Demands
                .Include(d => d.User)
                .Include(d => d.DemandItems)
                .Include(d => d.Status)
                .OrderBy(d => d.DemandId)
            };
            if (id != null)
            {
                ViewBag.DemandId = id.Value;
                model.DemandItems = model.Demands.Where(
                    d => d.DemandId == id.Value).Single().DemandItems;
            }
            return View(model);
        }

        // GET: Demands/Details/5
        public ActionResult Details(int? id)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Demand demand = db.Demands.Find(id);
            if (demand == null)
            {
                return HttpNotFound();
            }
            return View(demand);
        }

        // GET: Demands/Create
        // Display form to the user.
        public ActionResult Create()
        {
            return View();
        }

        //// POST: Demand/Create
        //// Accept the input from the user.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Demand demand)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                if (userId != null)
                {
                    demand.UserId = Int32.Parse(userId);
                    demand.StatusId = 1;
                    db.Demands.Add(demand);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(demand);
        }

        // GET: Demands/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Demand demand = db.Demands.Find(id);
            if (demand == null)
            {
                return HttpNotFound();
            }

            ViewBag.StatusId = new SelectList(db.DemandStatuses, "Id", "StatusName", demand.StatusId);
            return View(demand);
        }

        // POST: Demands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DemandId,SubmitDate,HoldingId,UserId,Surname,Rank,DateDespatched,DateDelivered")] Demand demand)
        {
            if (ModelState.IsValid)
            {
                db.Entry(demand).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StatusId = new SelectList(db.DemandStatuses, "Id", "StatusName", demand.StatusId);
            return View(demand);
        }

        // GET: Demands/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Demand demand = db.Demands.Find(id);
            if (demand == null)
            {
                return HttpNotFound();
            }
            return View(demand);
        }

        // POST: Demands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Demand demand = db.Demands.Find(id);
            db.Demands.Remove(demand);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //GET: Demands/Items/DemandId
        public ActionResult Items(int? id, DemandItemViewModel model, DemandItem item)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Demand demand = db.Demands.Find(id);
            if (demand == null)
            {
                return HttpNotFound();
            }
            model.Holdings = db.Holdings.ToList();
            model.Demand = demand;

            List<int> quantity = new List<int>();
            for (int i = 0; i <= 20; i++)
            {
                quantity.Add(i);
            }

            ViewBag.ItemQuantity = new SelectList(quantity, 0);

            return View(model);
        }

        //GET: Demands/AddItem/HoldingId;DemandId
        public ActionResult AddItem(DemandItem item, int? id, int? demandId, string qty)
        {
            if (ModelState.IsValid)
            {
                var addedItem = db.Holdings.Find(id);
                var demand = db.Demands.Find(demandId);
                var selectedQty = Convert.ToInt32(qty);
                

                item = new DemandItem(addedItem, demand, selectedQty)
                {
                    DemandItemId = item.DemandItemId,
                    DemandId = item.DemandId,
                    HoldingId = item.HoldingId,
                    //ItemQuantity = item.ItemQuantity
                };

                db.Entry(item).State = EntityState.Added;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
