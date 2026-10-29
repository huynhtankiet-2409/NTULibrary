using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectCK_65131559.Models;
using ProjectCK_65131559.Helpers;

namespace ProjectCK_65131559.Areas.Admin_65131559.Controllers
{
    public class NguoiDung_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Admin_65131559/NguoiDung_65131559
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PaginatedList<NguoiDung>.Create(db.NguoiDungs.OrderByDescending(n => n.NgayTao), pageNumber, pageSize));
        }

        // GET: Admin_65131559/NguoiDung_65131559/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        // GET: Admin_65131559/NguoiDung_65131559/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin_65131559/NguoiDung_65131559/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaNguoiDung,TenDangNhap,MaSoSinhVien,HoTen,Email,MatKhau,VaiTro,SoDienThoai,Khoa,TrangThai,AnhDaiDien")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                // Tự động set NgayTao và TrangThai mặc định
                nguoiDung.NgayTao = DateTime.Now;
                if (string.IsNullOrEmpty(nguoiDung.TrangThai))
                {
                    nguoiDung.TrangThai = "HoatDong";
                }

                db.NguoiDungs.Add(nguoiDung);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Tạo người dùng thành công!";
                return RedirectToAction("Index");
            }

            return View(nguoiDung);
        }

        // GET: Admin_65131559/NguoiDung_65131559/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        // POST: Admin_65131559/NguoiDung_65131559/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNguoiDung,TenDangNhap,MaSoSinhVien,HoTen,Email,MatKhau,VaiTro,SoDienThoai,Khoa,TrangThai,AnhDaiDien,NgayTao")] NguoiDung nguoiDung, string MatKhauMoi)
        {
            if (ModelState.IsValid)
            {
                // Lấy user hiện tại từ database
                var existingUser = db.NguoiDungs.Find(nguoiDung.MaNguoiDung);
                if (existingUser != null)
                {
                    // Cập nhật các trường
                    existingUser.TenDangNhap = nguoiDung.TenDangNhap;
                    existingUser.MaSoSinhVien = nguoiDung.MaSoSinhVien;
                    existingUser.HoTen = nguoiDung.HoTen;
                    existingUser.Email = nguoiDung.Email;
                    existingUser.VaiTro = nguoiDung.VaiTro;
                    existingUser.SoDienThoai = nguoiDung.SoDienThoai;
                    existingUser.Khoa = nguoiDung.Khoa;
                    existingUser.TrangThai = nguoiDung.TrangThai;
                    existingUser.AnhDaiDien = nguoiDung.AnhDaiDien;

                    // Chỉ cập nhật mật khẩu nếu có giá trị mới
                    if (!string.IsNullOrEmpty(MatKhauMoi))
                    {
                        existingUser.MatKhau = MatKhauMoi;
                    }

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Index");
                }
            }
            return View(nguoiDung);
        }

        // GET: Admin_65131559/NguoiDung_65131559/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        // POST: Admin_65131559/NguoiDung_65131559/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            db.NguoiDungs.Remove(nguoiDung);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Admin_65131559/NguoiDung_65131559/ToggleLock/5
        public ActionResult ToggleLock(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        // POST: Admin_65131559/NguoiDung_65131559/ToggleLock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleLockConfirmed(int id)
        {
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung != null)
            {
                // Toggle trạng thái
                if (nguoiDung.TrangThai == "HoatDong")
                {
                    nguoiDung.TrangThai = "BiKhoa";
                    TempData["SuccessMessage"] = $"Đã khóa tài khoản {nguoiDung.HoTen}";
                    
                    // Gửi email thông báo (nếu có email)
                    if (!string.IsNullOrEmpty(nguoiDung.Email))
                    {
                        EmailHelper.SendAccountLockedEmail(nguoiDung.Email, nguoiDung.HoTen);
                    }
                }
                else
                {
                    nguoiDung.TrangThai = "HoatDong";
                    TempData["SuccessMessage"] = $"Đã mở khóa tài khoản {nguoiDung.HoTen}";
                }

                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: Admin_65131559/NguoiDung_65131559/SendPasswordReset/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendPasswordReset(int id)
        {
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(nguoiDung.Email))
            {
                TempData["ErrorMessage"] = "Người dùng chưa có email";
                return RedirectToAction("Index");
            }

            // Tạo token reset mật khẩu
            string token = PasswordResetToken.GenerateToken(nguoiDung.MaNguoiDung, nguoiDung.Email);

            // Lấy base URL
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;

            // Gửi email
            bool emailSent = EmailHelper.SendPasswordResetEmail(
                nguoiDung.Email, 
                nguoiDung.HoTen, 
                token, 
                baseUrl
            );

            if (emailSent)
            {
                TempData["SuccessMessage"] = $"Đã gửi email reset mật khẩu đến {nguoiDung.Email}";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể gửi email. Vui lòng kiểm tra cấu hình SMTP";
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
