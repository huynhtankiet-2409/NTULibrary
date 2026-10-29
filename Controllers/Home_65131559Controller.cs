using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectCK_65131559.Models;
using System.Data.Entity;

namespace ProjectCK_65131559.Controllers
{
    public class Home_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        public ActionResult Index()
        {
            // Auto-redirect if logged in
            if (Session["VaiTro"] != null)
            {
                string vaiTro = Session["VaiTro"].ToString().ToLower();
                if (vaiTro.Contains("admin") || vaiTro.Contains("quantri"))
                {
                    return RedirectToAction("Index", "AdminHome_65131559", new { area = "Admin_65131559" });
                }
                else if (vaiTro.Contains("user") || vaiTro.Contains("nguoidung") || vaiTro.Contains("sinhvien") || vaiTro.Contains("canbo"))
                {
                    return RedirectToAction("Index", "UserHome_65131559", new { area = "User_65131559" });
                }
            }
            var viewModel = new HomeViewModel_65131559();

            // 1. New Arrivals (Latest 8)
            viewModel.NewArrivals = db.TaiLieux
                .Include(t => t.DanhMucCon)
                .OrderByDescending(t => t.NgayTao)
                .ThenByDescending(t => t.MaTaiLieu)
                .Take(8)
                .ToList();

            // 2. Trending Documents (Top 5 by Views + Downloads)
            viewModel.TrendingDocuments = db.TaiLieux
                .Include(t => t.DanhMucCon)
                .OrderByDescending(t => (t.LuotXem ?? 0) + (t.LuotTai ?? 0))
                .Take(5)
                .ToList();

            // 3. Category Tree
            viewModel.Categories = db.DanhMucChas
                .Include(c => c.DanhMucCons)
                .ToList();

            return View(viewModel);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}