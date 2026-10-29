using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectCK_65131559.Models;

namespace ProjectCK_65131559.Areas.User_65131559.Controllers
{
    [Authorize]
    public class DanhGia_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // POST: User_65131559/DanhGia_65131559/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(int maTaiLieu, int diemSao, string binhLuan)
        {
            var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == User.Identity.Name);
            if (user == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện bình luận!" });
            }

            if (diemSao < 1 || diemSao > 5)
            {
                return Json(new { success = false, message = "Vui lòng chọn số sao từ 1 đến 5!" });
            }

            if (string.IsNullOrWhiteSpace(binhLuan))
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung bình luận!" });
            }

            var review = new DanhGia
            {
                MaNguoiDung = user.MaNguoiDung,
                MaTaiLieu = maTaiLieu,
                DiemSao = diemSao,
                BinhLuan = binhLuan,
                DuocHienThi = true, // Default to true for user interaction
                NgayDang = DateTime.Now
            };

            try
            {
                db.DanhGias.Add(review);
                db.SaveChanges();
                return Json(new { 
                    success = true, 
                    message = "Cảm ơn bạn đã gửi đánh giá!",
                    review = new {
                        UserName = user.HoTen ?? user.TenDangNhap,
                        Content = review.BinhLuan,
                        Stars = review.DiemSao,
                        Date = review.NgayDang.Value.ToString("dd/MM/yyyy HH:mm")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
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
