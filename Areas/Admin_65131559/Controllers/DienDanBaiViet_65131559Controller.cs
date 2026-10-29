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
    public class DienDanBaiViet_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Admin_65131559/DienDanBaiViet_65131559
        public ActionResult Index(string status, int? page)
        {
            var query = db.DienDan_BaiViet.Include(d => d.DienDan_ChuyenMuc).Include(d => d.NguoiDung).AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(v => v.TrangThai == status);
            }

            ViewBag.CurrentStatus = status;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PaginatedList<DienDan_BaiViet>.Create(query.OrderByDescending(v => v.NgayDang), pageNumber, pageSize));
        }

        // GET: Admin_65131559/DienDanBaiViet_65131559/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DienDan_BaiViet dienDan_BaiViet = db.DienDan_BaiViet.Find(id);
            if (dienDan_BaiViet == null)
            {
                return HttpNotFound();
            }
            return View(dienDan_BaiViet);
        }

        // POST: Update status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int id, string status)
        {
            var post = db.DienDan_BaiViet.Find(id);
            if (post != null)
            {
                post.TrangThai = status;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: Toggle Pin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TogglePin(int id)
        {
            var post = db.DienDan_BaiViet.Find(id);
            if (post != null)
            {
                post.LaGhim = !(post.LaGhim ?? false);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: Admin_65131559/DienDanBaiViet_65131559/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DienDan_BaiViet dienDan_BaiViet = db.DienDan_BaiViet.Find(id);
            
            // Should properly clear related reports and comments or move to BiAn
            // For now, let's just move to BiAn as a "Soft Delete" if we want to retain history
            // or perform a hard delete if requested. The user said "Ẩn/Xóa: Chuyển sang BiAn".
            
            dienDan_BaiViet.TrangThai = "BiAn";
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
