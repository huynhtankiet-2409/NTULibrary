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
    public class DanhGia_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Admin_65131559/DanhGia_65131559
        public ActionResult Index(int? page)
        {
            var danhGias = db.DanhGias.Include(d => d.NguoiDung).Include(d => d.TaiLieu);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PaginatedList<DanhGia>.Create(danhGias.OrderByDescending(d => d.NgayDang), pageNumber, pageSize));
        }

        // GET: Admin_65131559/DanhGia_65131559/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhGia danhGia = db.DanhGias.Find(id);
            if (danhGia == null)
            {
                return HttpNotFound();
            }
            return View(danhGia);
        }

        // GET: Admin_65131559/DanhGia_65131559/Create
        public ActionResult Create()
        {
            ViewBag.MaNguoiDung = new SelectList(db.NguoiDungs, "MaNguoiDung", "TenDangNhap");
            ViewBag.MaTaiLieu = new SelectList(db.TaiLieux, "MaTaiLieu", "TenTaiLieu");
            return View();
        }

        // POST: Admin_65131559/DanhGia_65131559/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaDanhGia,MaNguoiDung,MaTaiLieu,DiemSao,BinhLuan,DuocHienThi,NgayDang")] DanhGia danhGia)
        {
            if (ModelState.IsValid)
            {
                db.DanhGias.Add(danhGia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaNguoiDung = new SelectList(db.NguoiDungs, "MaNguoiDung", "TenDangNhap", danhGia.MaNguoiDung);
            ViewBag.MaTaiLieu = new SelectList(db.TaiLieux, "MaTaiLieu", "TenTaiLieu", danhGia.MaTaiLieu);
            return View(danhGia);
        }

        // GET: Admin_65131559/DanhGia_65131559/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhGia danhGia = db.DanhGias.Find(id);
            if (danhGia == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaNguoiDung = new SelectList(db.NguoiDungs, "MaNguoiDung", "TenDangNhap", danhGia.MaNguoiDung);
            ViewBag.MaTaiLieu = new SelectList(db.TaiLieux, "MaTaiLieu", "TenTaiLieu", danhGia.MaTaiLieu);
            return View(danhGia);
        }

        // POST: Admin_65131559/DanhGia_65131559/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaDanhGia,MaNguoiDung,MaTaiLieu,DiemSao,BinhLuan,DuocHienThi,NgayDang")] DanhGia danhGia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(danhGia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaNguoiDung = new SelectList(db.NguoiDungs, "MaNguoiDung", "TenDangNhap", danhGia.MaNguoiDung);
            ViewBag.MaTaiLieu = new SelectList(db.TaiLieux, "MaTaiLieu", "TenTaiLieu", danhGia.MaTaiLieu);
            return View(danhGia);
        }

        // GET: Admin_65131559/DanhGia_65131559/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhGia danhGia = db.DanhGias.Find(id);
            if (danhGia == null)
            {
                return HttpNotFound();
            }
            return View(danhGia);
        }

        // POST: Admin_65131559/DanhGia_65131559/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DanhGia danhGia = db.DanhGias.Find(id);
            db.DanhGias.Remove(danhGia);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Admin_65131559/DanhGia_65131559/ToggleVisibility/5
        public ActionResult ToggleVisibility(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhGia danhGia = db.DanhGias.Find(id);
            if (danhGia == null)
            {
                return HttpNotFound();
            }
            // Toggle status. If null, treat as hidden (false) -> become visible (true)
            // Or if we want default visible, treat null as true. 
            // Let's assume null means "not set" but effectively visible or pending.
            // Requirement says: "Admin hides negative comments" -> implies default might be visible.
            // Let's safe toggle: if true -> false, if false/null -> true.
            danhGia.DuocHienThi = !(danhGia.DuocHienThi ?? false);
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
