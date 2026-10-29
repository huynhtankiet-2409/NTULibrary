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
    public class LichSuTruyCap_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Admin_65131559/LichSuTruyCap_65131559
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        // GET: Admin_65131559/LichSuTruyCap_65131559/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LichSuTruyCap lichSuTruyCap = db.LichSuTruyCaps.Find(id);
            if (lichSuTruyCap == null)
            {
                return HttpNotFound();
            }
            return View(lichSuTruyCap);
        }

        // GET: Admin_65131559/LichSuTruyCap_65131559/Create
        public ActionResult Create()
        {
            ViewBag.MaNguoiDung = new SelectList(db.NguoiDungs, "MaNguoiDung", "TenDangNhap");
            ViewBag.MaTaiLieu = new SelectList(db.TaiLieux, "MaTaiLieu", "TenTaiLieu");
            return View();
        }

        // POST: Admin_65131559/LichSuTruyCap_65131559/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaLichSu,MaNguoiDung,MaTaiLieu,ThoiGian,HanhDong")] LichSuTruyCap lichSuTruyCap)
        {
            if (ModelState.IsValid)
            {
                db.LichSuTruyCaps.Add(lichSuTruyCap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaNguoiDung = new SelectList(db.NguoiDungs, "MaNguoiDung", "TenDangNhap", lichSuTruyCap.MaNguoiDung);
            ViewBag.MaTaiLieu = new SelectList(db.TaiLieux, "MaTaiLieu", "TenTaiLieu", lichSuTruyCap.MaTaiLieu);
            return View(lichSuTruyCap);
        }

        // GET: Admin_65131559/LichSuTruyCap_65131559/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LichSuTruyCap lichSuTruyCap = db.LichSuTruyCaps.Find(id);
            if (lichSuTruyCap == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaNguoiDung = new SelectList(db.NguoiDungs, "MaNguoiDung", "TenDangNhap", lichSuTruyCap.MaNguoiDung);
            ViewBag.MaTaiLieu = new SelectList(db.TaiLieux, "MaTaiLieu", "TenTaiLieu", lichSuTruyCap.MaTaiLieu);
            return View(lichSuTruyCap);
        }

        // POST: Admin_65131559/LichSuTruyCap_65131559/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaLichSu,MaNguoiDung,MaTaiLieu,ThoiGian,HanhDong")] LichSuTruyCap lichSuTruyCap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lichSuTruyCap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaNguoiDung = new SelectList(db.NguoiDungs, "MaNguoiDung", "TenDangNhap", lichSuTruyCap.MaNguoiDung);
            ViewBag.MaTaiLieu = new SelectList(db.TaiLieux, "MaTaiLieu", "TenTaiLieu", lichSuTruyCap.MaTaiLieu);
            return View(lichSuTruyCap);
        }

        // GET: Admin_65131559/LichSuTruyCap_65131559/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LichSuTruyCap lichSuTruyCap = db.LichSuTruyCaps.Find(id);
            if (lichSuTruyCap == null)
            {
                return HttpNotFound();
            }
            return View(lichSuTruyCap);
        }

        // POST: Admin_65131559/LichSuTruyCap_65131559/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LichSuTruyCap lichSuTruyCap = db.LichSuTruyCaps.Find(id);
            db.LichSuTruyCaps.Remove(lichSuTruyCap);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Admin_65131559/LichSuTruyCap_65131559/Dashboard
        public ActionResult Dashboard()
        {
            // 1. Get Overview Stats (Base model)
            var stats = db.sp_LayThongKeDashboard().FirstOrDefault();
            ViewBag.Stats = stats;

            // 2. Supplement with calculated stats
            ViewBag.TongLuotXem = db.TaiLieux.Sum(t => t.LuotXem) ?? 0;
            ViewBag.TongLuotTai = db.TaiLieux.Sum(t => t.LuotTai) ?? 0;

            // 3. Get Trending Documents (Top 5 by Views + Downloads)
            var trendingDocs = db.TaiLieux
                .Include(t => t.DanhMucCon)
                .OrderByDescending(t => (t.LuotXem ?? 0) + (t.LuotTai ?? 0))
                .Take(5)
                .ToList();

            return View(trendingDocs);
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
