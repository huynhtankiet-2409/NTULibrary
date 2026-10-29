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
    public class DienDanBaoCao_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Admin_65131559/DienDanBaoCao_65131559
        public ActionResult Index(int? page)
        {
            var reports = db.DienDan_BaoCao
                .Include(d => d.DienDan_BaiViet)
                .Include(d => d.NguoiDung)
                .OrderByDescending(d => d.NgayBaoCao);
                
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PaginatedList<DienDan_BaoCao>.Create(reports, pageNumber, pageSize));
        }

        // POST: Handle Report
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Handle(int id, string actionType)
        {
            var report = db.DienDan_BaoCao.Include(r => r.DienDan_BaiViet).FirstOrDefault(r => r.MaBaoCao == id);
            
            if (report != null)
            {
                if (actionType == "Hide")
                {
                    // Hide the reported post
                    report.DienDan_BaiViet.TrangThai = "BiAn";
                    report.TrangThaiXuLy = "DaXuLy - AnBaiViet";
                }
                else if (actionType == "Dismiss")
                {
                    // Report is false/spam, just mark as resolved without action
                    report.TrangThaiXuLy = "DaXuLy - BoQua";
                }
                
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
