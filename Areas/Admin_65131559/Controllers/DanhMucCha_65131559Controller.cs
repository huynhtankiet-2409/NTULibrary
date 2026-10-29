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
    public class DanhMucCha_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Admin_65131559/DanhMucCha_65131559
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PaginatedList<DanhMucCha>.Create(db.DanhMucChas.OrderBy(d => d.MaDanhMucCha), pageNumber, pageSize));
        }

        // GET: Admin_65131559/DanhMucCha_65131559/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMucCha danhMucCha = db.DanhMucChas.Find(id);
            if (danhMucCha == null)
            {
                return HttpNotFound();
            }
            return View(danhMucCha);
        }

        // GET: Admin_65131559/DanhMucCha_65131559/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin_65131559/DanhMucCha_65131559/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaDanhMucCha,TenDanhMucCha,MoTa")] DanhMucCha danhMucCha)
        {
            if (ModelState.IsValid)
            {
                db.DanhMucChas.Add(danhMucCha);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(danhMucCha);
        }

        // GET: Admin_65131559/DanhMucCha_65131559/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMucCha danhMucCha = db.DanhMucChas.Find(id);
            if (danhMucCha == null)
            {
                return HttpNotFound();
            }
            return View(danhMucCha);
        }

        // POST: Admin_65131559/DanhMucCha_65131559/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaDanhMucCha,TenDanhMucCha,MoTa")] DanhMucCha danhMucCha)
        {
            if (ModelState.IsValid)
            {
                db.Entry(danhMucCha).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(danhMucCha);
        }

        // GET: Admin_65131559/DanhMucCha_65131559/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMucCha danhMucCha = db.DanhMucChas.Find(id);
            if (danhMucCha == null)
            {
                return HttpNotFound();
            }
            return View(danhMucCha);
        }

        // POST: Admin_65131559/DanhMucCha_65131559/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DanhMucCha danhMucCha = db.DanhMucChas.Find(id);
            db.DanhMucChas.Remove(danhMucCha);
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
