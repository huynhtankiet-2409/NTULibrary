using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectCK_65131559.Models;

namespace ProjectCK_65131559.Areas.Admin_65131559.Controllers
{
    public class DanhMucCon_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Admin_65131559/DanhMucCon_65131559
        public ActionResult Index(int? page)
        {
            var danhMucCons = db.DanhMucCons.Include(d => d.DanhMucCha);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PaginatedList<DanhMucCon>.Create(danhMucCons.OrderBy(d => d.MaDanhMucCon), pageNumber, pageSize));
        }

        // GET: Admin_65131559/DanhMucCon_65131559/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMucCon danhMucCon = db.DanhMucCons.Find(id);
            if (danhMucCon == null)
            {
                return HttpNotFound();
            }
            return View(danhMucCon);
        }

        // GET: Admin_65131559/DanhMucCon_65131559/Create
        public ActionResult Create()
        {
            ViewBag.MaDanhMucCha = new SelectList(db.DanhMucChas, "MaDanhMucCha", "TenDanhMucCha");
            return View();
        }

        // POST: Admin_65131559/DanhMucCon_65131559/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaDanhMucCon,TenDanhMucCon,MaDanhMucCha")] DanhMucCon danhMucCon)
        {
            if (ModelState.IsValid)
            {
                db.DanhMucCons.Add(danhMucCon);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDanhMucCha = new SelectList(db.DanhMucChas, "MaDanhMucCha", "TenDanhMucCha", danhMucCon.MaDanhMucCha);
            return View(danhMucCon);
        }

        // GET: Admin_65131559/DanhMucCon_65131559/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMucCon danhMucCon = db.DanhMucCons.Find(id);
            if (danhMucCon == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaDanhMucCha = new SelectList(db.DanhMucChas, "MaDanhMucCha", "TenDanhMucCha", danhMucCon.MaDanhMucCha);
            return View(danhMucCon);
        }

        // POST: Admin_65131559/DanhMucCon_65131559/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaDanhMucCon,TenDanhMucCon,MaDanhMucCha")] DanhMucCon danhMucCon)
        {
            if (ModelState.IsValid)
            {
                db.Entry(danhMucCon).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaDanhMucCha = new SelectList(db.DanhMucChas, "MaDanhMucCha", "TenDanhMucCha", danhMucCon.MaDanhMucCha);
            return View(danhMucCon);
        }

        // GET: Admin_65131559/DanhMucCon_65131559/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMucCon danhMucCon = db.DanhMucCons.Find(id);
            if (danhMucCon == null)
            {
                return HttpNotFound();
            }
            return View(danhMucCon);
        }

        // POST: Admin_65131559/DanhMucCon_65131559/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DanhMucCon danhMucCon = db.DanhMucCons.Find(id);
            db.DanhMucCons.Remove(danhMucCon);
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
