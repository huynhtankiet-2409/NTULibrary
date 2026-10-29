using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectCK_65131559.Models;

namespace ProjectCK_65131559.Areas.User_65131559.Controllers
{
    [Authorize]
    public class TuSach_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: User_65131559/TuSach_65131559
        public ActionResult Index()
        {
            var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == User.Identity.Name);
            if (user == null) return RedirectToAction("Login", "Account_65131559", new { area = "" });

            var favorites = db.TaiLieuYeuThiches
                .Where(t => t.MaNguoiDung == user.MaNguoiDung)
                .Include(t => t.TaiLieu)
                .Include(t => t.TaiLieu.DanhMucCon)
                .OrderByDescending(t => t.NgayThem)
                .ToList();

            return View(favorites);
        }

        // POST: User_65131559/TuSach_65131559/Toggle
        // Adds if not exists, Removes if exists
        [HttpPost]
        public ActionResult Toggle(int id)
        {
            var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == User.Identity.Name);
            if (user == null) return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện chức năng này!" });

            var exists = db.TaiLieuYeuThiches.FirstOrDefault(t => t.MaNguoiDung == user.MaNguoiDung && t.MaTaiLieu == id);

            if (exists != null)
            {
                // Remove
                db.TaiLieuYeuThiches.Remove(exists);
                db.SaveChanges();
                return Json(new { success = true, isAdded = false, message = "Đã xóa khỏi tủ sách!" });
            }
            else
            {
                // Add
                var newItem = new TaiLieuYeuThich
                {
                    MaNguoiDung = user.MaNguoiDung,
                    MaTaiLieu = id,
                    NgayThem = DateTime.Now
                };
                db.TaiLieuYeuThiches.Add(newItem);
                db.SaveChanges();
                return Json(new { success = true, isAdded = true, message = "Đã thêm vào tủ sách!" });
            }
        }

        // POST: User_65131559/TuSach_65131559/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int maTaiLieu)
        {
            var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == User.Identity.Name);
            if (user == null) return RedirectToAction("Login", "Account_65131559", new { area = "" });

            var item = db.TaiLieuYeuThiches.FirstOrDefault(t => t.MaNguoiDung == user.MaNguoiDung && t.MaTaiLieu == maTaiLieu);
            if (item != null)
            {
                db.TaiLieuYeuThiches.Remove(item);
                db.SaveChanges();
            }
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
