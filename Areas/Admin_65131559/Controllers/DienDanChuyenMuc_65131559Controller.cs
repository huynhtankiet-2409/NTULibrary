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
    [Authorize]
    public class DienDanChuyenMuc_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Admin_65131559/DienDanChuyenMuc_65131559
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PaginatedList<DienDan_ChuyenMuc>.Create(db.DienDan_ChuyenMuc.OrderBy(x => x.ThuTuHienThi), pageNumber, pageSize));
        }

        // GET: Admin_65131559/DienDanChuyenMuc_65131559/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin_65131559/DienDanChuyenMuc_65131559/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaChuyenMuc,TenChuyenMuc,MoTa,ThuTuHienThi")] DienDan_ChuyenMuc dienDan_ChuyenMuc)
        {
            if (ModelState.IsValid)
            {
                db.DienDan_ChuyenMuc.Add(dienDan_ChuyenMuc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dienDan_ChuyenMuc);
        }

        // GET: Admin_65131559/DienDanChuyenMuc_65131559/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DienDan_ChuyenMuc dienDan_ChuyenMuc = db.DienDan_ChuyenMuc.Find(id);
            if (dienDan_ChuyenMuc == null)
            {
                return HttpNotFound();
            }
            return View(dienDan_ChuyenMuc);
        }

        // POST: Admin_65131559/DienDanChuyenMuc_65131559/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaChuyenMuc,TenChuyenMuc,MoTa,ThuTuHienThi")] DienDan_ChuyenMuc dienDan_ChuyenMuc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dienDan_ChuyenMuc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dienDan_ChuyenMuc);
        }

        // GET: Admin_65131559/DienDanChuyenMuc_65131559/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DienDan_ChuyenMuc dienDan_ChuyenMuc = db.DienDan_ChuyenMuc.Find(id);
            if (dienDan_ChuyenMuc == null)
            {
                return HttpNotFound();
            }
            return View(dienDan_ChuyenMuc);
        }

        // POST: Admin_65131559/DienDanChuyenMuc_65131559/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DienDan_ChuyenMuc dienDan_ChuyenMuc = db.DienDan_ChuyenMuc.Find(id);
            
            // Check for related posts
            if (dienDan_ChuyenMuc.DienDan_BaiViet.Any())
            {
                ModelState.AddModelError("", "Không thể xóa chuyên mục này vì đã có bài viết bên trong.");
                return View(dienDan_ChuyenMuc);
            }

            db.DienDan_ChuyenMuc.Remove(dienDan_ChuyenMuc);
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
