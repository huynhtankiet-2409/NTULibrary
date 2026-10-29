using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectCK_65131559.Models;

namespace ProjectCK_65131559.Areas.Admin_65131559.Controllers
{
    [Authorize]
    public class YeuCau_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Admin_65131559/YeuCau_65131559
        public ActionResult Index(string trangThai, int? page)
        {
            var requests = db.YeuCaus.Include(y => y.NguoiDung).OrderByDescending(y => y.NgayGui).AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
            {
                requests = requests.Where(y => y.TrangThai == trangThai);
            }

            ViewBag.CurrentTrangThai = trangThai;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PaginatedList<YeuCau>.Create(requests, pageNumber, pageSize));
        }

        // GET: Details
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            YeuCau yeuCau = db.YeuCaus.Include(y => y.NguoiDung).FirstOrDefault(y => y.MaYeuCau == id);
            if (yeuCau == null) return HttpNotFound();
            return View(yeuCau);
        }

        // POST: Link to process the request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Handle(int MaYeuCau, string PhanHoiTuAdmin, string TrangThai)
        {
            var yeuCau = db.YeuCaus.Find(MaYeuCau);
            if (yeuCau == null) return HttpNotFound();

            yeuCau.PhanHoiTuAdmin = PhanHoiTuAdmin;
            yeuCau.TrangThai = TrangThai;
            yeuCau.NgayXuLy = DateTime.Now;

            db.Entry(yeuCau).State = EntityState.Modified;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đã cập nhật phản hồi cho yêu cầu #" + MaYeuCau;
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
