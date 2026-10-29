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
    public class TacGia_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Admin_65131559/TacGia_65131559
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PaginatedList<TacGia>.Create(db.TacGias.OrderBy(t => t.HoTen), pageNumber, pageSize));
        }

        // GET: Admin_65131559/TacGia_65131559/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TacGia tacGia = db.TacGias.Find(id);
            if (tacGia == null)
            {
                return HttpNotFound();
            }
            return View(tacGia);
        }

        // GET: Admin_65131559/TacGia_65131559/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin_65131559/TacGia_65131559/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaTacGia,HoTen,GioiThieu")] TacGia tacGia)
        {
            if (ModelState.IsValid)
            {
                db.TacGias.Add(tacGia);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm tác giả thành công!";
                return RedirectToAction("Index");
            }

            return View(tacGia);
        }

        // GET: Admin_65131559/TacGia_65131559/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TacGia tacGia = db.TacGias.Find(id);
            if (tacGia == null)
            {
                return HttpNotFound();
            }
            return View(tacGia);
        }

        // POST: Admin_65131559/TacGia_65131559/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaTacGia,HoTen,GioiThieu")] TacGia tacGia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tacGia).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật tác giả thành công!";
                return RedirectToAction("Index");
            }
            return View(tacGia);
        }

        // GET: Admin_65131559/TacGia_65131559/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TacGia tacGia = db.TacGias.Find(id);
            if (tacGia == null)
            {
                return HttpNotFound();
            }
            return View(tacGia);
        }

        // POST: Admin_65131559/TacGia_65131559/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TacGia tacGia = db.TacGias.Find(id);
            // Check if author has documents?
            // Usually we might want to check referencing integrity, but EF might throw or cascade.
            // For now, simple delete.
            db.TacGias.Remove(tacGia);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Xóa tác giả thành công!";
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
