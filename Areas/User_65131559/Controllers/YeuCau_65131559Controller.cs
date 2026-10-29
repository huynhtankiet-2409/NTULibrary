using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectCK_65131559.Models;
using System.Data.Entity;

namespace ProjectCK_65131559.Areas.User_65131559.Controllers
{
    [Authorize]
    public class YeuCau_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: User_65131559/YeuCau_65131559
        public ActionResult Index()
        {
            var username = User.Identity.Name;
            var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == username);
            if (user == null) return HttpNotFound();

            var requests = db.YeuCaus
                .Where(r => r.MaNguoiDung == user.MaNguoiDung)
                .OrderByDescending(r => r.NgayGui)
                .ToList();

            return View(requests);
        }

        // GET: Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(YeuCau yeuCau)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == username);
                if (user != null)
                {
                    yeuCau.MaNguoiDung = user.MaNguoiDung;
                    yeuCau.NgayGui = DateTime.Now;
                    yeuCau.TrangThai = "ChoXuLy";
                    
                    db.YeuCaus.Add(yeuCau);
                    db.SaveChanges();
                    
                    TempData["SuccessMessage"] = "Yêu cầu của bạn đã được gửi thành công!";
                    return RedirectToAction("Index");
                }
            }
            return View(yeuCau);
        }

        // GET: Details
        public ActionResult Details(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            
            var username = User.Identity.Name;
            var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == username);
            
            var yeuCau = db.YeuCaus.Find(id);
            if (yeuCau == null || yeuCau.MaNguoiDung != user.MaNguoiDung) return HttpNotFound();
            
            return View(yeuCau);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
