using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectCK_65131559.Models;
using System.Data.Entity;

namespace ProjectCK_65131559.Areas.User_65131559.Controllers
{
    public class UserHome_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: User_65131559/UserHome_65131559
        public ActionResult Index()
        {
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
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

namespace ProjectCK_65131559.Models
{
    public class HomeViewModel_65131559
    {
        public List<TaiLieu> NewArrivals { get; set; }
        public List<TaiLieu> TrendingDocuments { get; set; }
        public List<DanhMucCha> Categories { get; set; }
    }
}