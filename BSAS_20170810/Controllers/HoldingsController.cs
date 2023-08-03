using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BSAS_20170810.Models;
using System.Diagnostics;

namespace BSAS_20170810.Controllers
{
    //[Authorize]
    public class HoldingsController : Controller
    {
        private BSASDbContext db = new BSASDbContext();

        // GET: Holdings
        //
        public ActionResult Index(string searchString)
        {
            var holdings = db.Holdings.Include(h => h.Category);

            if (!String.IsNullOrEmpty(searchString))
            {
                holdings = holdings.Where(h => h.HoldingName.Contains(searchString));
            }
            return View(holdings.ToList());
        }

        // GET: Holdings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Holding holding = db.Holdings.Find(id);
            if (holding == null)
            {
                return HttpNotFound();
            }
            return View(holding);
        }

        // GET: Holdings/Create
        //[Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.HoldingCategories, "Id", "CategoryName");
            return View();
        }

        // POST: Holdings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HoldingId,NSN,HoldingName,CategoryId,Description,Quantity,PartNumber")] Holding holding)
        {
            if (ModelState.IsValid)
            {
                db.Holdings.Add(holding);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.HoldingCategories, "Id", "CategoryName", holding.CategoryId);
            return View(holding);
        }

        // GET: Holdings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Holding holding = db.Holdings.Find(id);
            if (holding == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.HoldingCategories, "Id", "CategoryName", holding.CategoryId);
            return View(holding);
        }

        // POST: Holdings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HoldingId,NSN,HoldingName,CategoryId,Description,Quantity,PartNumber")] Holding holding)
        {
            if (ModelState.IsValid)
            {
                db.Entry(holding).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.HoldingCategories, "Id", "CategoryName", holding.CategoryId);
            return View(holding);
        }

        // GET: Holdings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Holding holding = db.Holdings.Find(id);
            if (holding == null)
            {
                return HttpNotFound();
            }
            return View(holding);
        }

        // POST: Holdings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Holding holding = db.Holdings.Find(id);
            db.Holdings.Remove(holding);
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
    }
}
