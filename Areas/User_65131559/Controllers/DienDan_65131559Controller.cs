using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectCK_65131559.Models;

namespace ProjectCK_65131559.Areas.User_65131559.Controllers
{
    [Authorize]
    public class DienDan_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: User_65131559/DienDan_65131559
        public ActionResult Index()
        {
            var categories = db.DienDan_ChuyenMuc.OrderBy(x => x.ThuTuHienThi).ToList();
            var latestPosts = db.DienDan_BaiViet.Include(b => b.NguoiDung).Include(b => b.DienDan_ChuyenMuc)
                                .Where(b => b.TrangThai == "HienThi")
                                .OrderByDescending(b => b.NgayDang)
                                .Take(5).ToList();
            
            ViewBag.LatestPosts = latestPosts;
            return View(categories);
        }

        // GET: Category view
        public ActionResult ChuyenMuc(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            
            var category = db.DienDan_ChuyenMuc.Find(id);
            if (category == null) return HttpNotFound();
            
            var posts = db.DienDan_BaiViet.Include(b => b.NguoiDung)
                            .Where(b => b.MaChuyenMuc == id && b.TrangThai == "HienThi")
                            .OrderByDescending(b => b.LaGhim)
                            .ThenByDescending(b => b.NgayDang)
                            .ToList();
            
            ViewBag.Category = category;
            return View(posts);
        }

        // GET: Article detail
        public ActionResult BaiViet(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            
            var article = db.DienDan_BaiViet.Include(b => b.NguoiDung).Include(b => b.DienDan_ChuyenMuc)
                            .Include(b => b.DienDan_BinhLuan)
                            .Include("DienDan_BinhLuan.NguoiDung")
                            .FirstOrDefault(b => b.MaBaiViet == id);
            
            if (article == null || (article.TrangThai != "HienThi" && article.TrangThai != "BiKhoa" && article.NguoiDung.TenDangNhap != User.Identity.Name))
                return HttpNotFound();

            // Increment View count
            article.LuotXem = (article.LuotXem ?? 0) + 1;
            db.SaveChanges();

            return View(article);
        }

        // GET: Create topic
        public ActionResult Create(int? categoryId)
        {
            ViewBag.MaChuyenMuc = new SelectList(db.DienDan_ChuyenMuc, "MaChuyenMuc", "TenChuyenMuc", categoryId);
            return View();
        }

        // POST: Create topic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DienDan_BaiViet baiViet)
        {
            if (ModelState.IsValid)
            {
                var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == User.Identity.Name);
                if (user != null)
                {
                    baiViet.MaNguoiDung = user.MaNguoiDung;
                    baiViet.NgayDang = DateTime.Now;
                    baiViet.LuotXem = 0;
                    baiViet.TrangThai = "HienThi"; // Default to visible in student area, or "ChoDuyet" if moderation is forced
                    baiViet.LaGhim = false;
                    
                    db.DienDan_BaiViet.Add(baiViet);
                    db.SaveChanges();
                    return RedirectToAction("BaiViet", new { id = baiViet.MaBaiViet });
                }
            }

            ViewBag.MaChuyenMuc = new SelectList(db.DienDan_ChuyenMuc, "MaChuyenMuc", "TenChuyenMuc", baiViet.MaChuyenMuc);
            return View(baiViet);
        }

        // POST: Reply/Comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(int maBaiViet, string noiDung)
        {
            var article = db.DienDan_BaiViet.Find(maBaiViet);
            if (article == null || article.TrangThai == "BiKhoa") 
                return Json(new { success = false, message = "Chủ đề này đã bị khóa hoặc không tồn tại." });

            if (string.IsNullOrWhiteSpace(noiDung))
                return Json(new { success = false, message = "Nội dung không được để trống." });

            var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == User.Identity.Name);
            if (user != null)
            {
                var reply = new DienDan_BinhLuan
                {
                    MaBaiViet = maBaiViet,
                    MaNguoiDung = user.MaNguoiDung,
                    NoiDung = noiDung,
                    NgayBinhLuan = DateTime.Now,
                    DuocHienThi = true
                };
                db.DienDan_BinhLuan.Add(reply);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Lỗi xác thực." });
        }

        // GET: Edit personal post
        public ActionResult Edit(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            var article = db.DienDan_BaiViet.Find(id);
            if (article == null || article.NguoiDung.TenDangNhap != User.Identity.Name) return HttpNotFound();
            
            ViewBag.MaChuyenMuc = new SelectList(db.DienDan_ChuyenMuc, "MaChuyenMuc", "TenChuyenMuc", article.MaChuyenMuc);
            return View(article);
        }

        // POST: Edit personal post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DienDan_BaiViet baiViet)
        {
            var existing = db.DienDan_BaiViet.Find(baiViet.MaBaiViet);
            if (existing == null || existing.NguoiDung.TenDangNhap != User.Identity.Name) return HttpNotFound();

            if (ModelState.IsValid)
            {
                existing.TieuDe = baiViet.TieuDe;
                existing.NoiDung = baiViet.NoiDung;
                existing.MaChuyenMuc = baiViet.MaChuyenMuc;
                db.SaveChanges();
                return RedirectToAction("BaiViet", new { id = existing.MaBaiViet });
            }

            ViewBag.MaChuyenMuc = new SelectList(db.DienDan_ChuyenMuc, "MaChuyenMuc", "TenChuyenMuc", baiViet.MaChuyenMuc);
            return View(baiViet);
        }

        // POST: Delete personal post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var article = db.DienDan_BaiViet.Include(b => b.DienDan_BinhLuan).FirstOrDefault(b => b.MaBaiViet == id);
            if (article == null || article.NguoiDung.TenDangNhap != User.Identity.Name) return HttpNotFound();
            
            // Check if post has interactions (comments)
            if (article.DienDan_BinhLuan.Any())
            {
                return Json(new { success = false, message = "Không thể xóa bài viết đã có bình luận. Vui lòng liên hệ quản trị viên." });
            }

            db.DienDan_BaiViet.Remove(article);
            db.SaveChanges();
            return Json(new { success = true });
        }

        // GET: Report view
        public ActionResult Report(int id)
        {
            var article = db.DienDan_BaiViet.Find(id);
            if (article == null) return HttpNotFound();
            return View(article);
        }

        // POST: Send report
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendReport(int maBaiViet, string lyDo)
        {
            var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == User.Identity.Name);
            if (user != null)
            {
                var report = new DienDan_BaoCao
                {
                    MaBaiViet = maBaiViet,
                    MaNguoiDungBaoCao = user.MaNguoiDung,
                    LyDo = lyDo,
                    NgayBaoCao = DateTime.Now,
                    TrangThaiXuLy = null
                };
                db.DienDan_BaoCao.Add(report);
                db.SaveChanges();
                return Json(new { success = true, message = "Báo cáo của bạn đã được gửi. Cảm ơn bạn!" });
            }
            return Json(new { success = false, message = "Lỗi xác thực." });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
