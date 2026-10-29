using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ProjectCK_65131559.Models;

namespace ProjectCK_65131559.Areas.Admin_65131559.Controllers
{
    public class TaiLieuYeuThich_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Admin_65131559/TaiLieuYeuThich_65131559
        // Analytics Dashboard
        public ActionResult Index()
        {
            // 1. INTEREST ANALYTICS: Top documents being saved/favorited
            // Compare Favorites (Potential) vs Downloads (Action)
            var interestData = db.TaiLieuYeuThiches
                .Include(t => t.TaiLieu)
                .GroupBy(t => t.TaiLieu)
                .Select(g => new
                {
                    TaiLieu = g.Key,
                    Favorites = g.Count()
                })
                .ToList() // Bring to memory to project into ViewModel
                .Select(x => new InterestMetric
                {
                    MaTaiLieu = x.TaiLieu.MaTaiLieu,
                    TenTaiLieu = x.TaiLieu.TenTaiLieu,
                    DanhMuc = x.TaiLieu.DanhMucCon != null ? x.TaiLieu.DanhMucCon.TenDanhMucCon : "Khác",
                    LuotThich = x.Favorites,
                    LuotXem = x.TaiLieu.LuotXem ?? 0,
                    LuotTai = x.TaiLieu.LuotTai ?? 0,
                    // Ratio: If Downloads < Favorites, High Potential but barrier exists?
                    InterestScore = (x.Favorites * 2) + (x.TaiLieu.LuotXem ?? 0) * 0.5 // Weighted score example
                })
                .OrderByDescending(x => x.LuotThich)
                .Take(20)
                .ToList();

            ViewBag.InterestAnalytics = interestData;

            // 2. USER INSIGHTS: Breakdown by Department (Khoa)
            // Helps identifying which Department uses the library most
            var deptInsights = db.TaiLieuYeuThiches
                .Include(t => t.NguoiDung)
                .Where(t => t.NguoiDung.Khoa != null)
                .GroupBy(t => t.NguoiDung.Khoa)
                .Select(g => new DepartmentInsight
                {
                    Khoa = g.Key,
                    SoLuongQuanTam = g.Count()
                })
                .OrderByDescending(x => x.SoLuongQuanTam)
                .ToList();

            ViewBag.DepartmentInsights = deptInsights;

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // View Models
        public class InterestMetric
        {
            public int MaTaiLieu { get; set; }
            public string TenTaiLieu { get; set; }
            public string DanhMuc { get; set; }
            public int LuotThich { get; set; }
            public int LuotXem { get; set; }
            public int LuotTai { get; set; }
            public double InterestScore { get; set; }
        }

        public class DepartmentInsight
        {
            public string Khoa { get; set; }
            public int SoLuongQuanTam { get; set; }
        }
    }
}
