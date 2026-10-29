using ProjectCK_65131559.Models;
using ProjectCK_65131559.Models.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;

namespace ProjectCK_65131559.Controllers
{
    public class Account_65131559Controller : Controller
    {
        private ThuVienDaiHocNhaTrangEntities db = new ThuVienDaiHocNhaTrangEntities();

        // GET: Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Tìm người dùng trong database
                var user = db.NguoiDungs.FirstOrDefault(u => 
                    u.TenDangNhap == model.Username && 
                    u.MatKhau == model.Password);

                if (user != null)
                {
                    // Tạo authentication ticket (session-only, expires on browser close)
                    FormsAuthentication.SetAuthCookie(user.TenDangNhap, false);

                    // Lưu thông tin vào Session
                    Session["MaNguoiDung"] = user.MaNguoiDung;
                    Session["TenDangNhap"] = user.TenDangNhap;
                    Session["HoTen"] = user.HoTen;
                    Session["VaiTro"] = user.VaiTro;

                    // 1. Kiểm tra returnUrl trước (ưu tiên quay lại trang cũ)
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // 2. Nếu không có returnUrl, redirect dựa trên vai trò (default)
                    string vaiTro = (user.VaiTro ?? "").Trim();
                    
                    // Check Admin role (case-insensitive)
                    if (vaiTro.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
                        vaiTro.Equals("QuanTriVien", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "AdminHome_65131559", new { area = "Admin_65131559" });
                    }
                    // Check User roles (case-insensitive)
                    else if (vaiTro.Equals("User", StringComparison.OrdinalIgnoreCase) || 
                             vaiTro.Equals("SinhVien", StringComparison.OrdinalIgnoreCase) || 
                             vaiTro.Equals("CanBo", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "UserHome_65131559", new { area = "User_65131559" });
                    }

                    return RedirectToAction("Index", "Home_65131559");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa.");
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // GET: ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                // Tìm người dùng theo email
                var user = db.NguoiDungs.FirstOrDefault(u => u.Email == model.Email);

                if (user != null)
                {
                    // TODO: Gửi email với mật khẩu mới hoặc link reset password
                    // Hiện tại chỉ hiển thị thông báo thành công
                    ViewBag.Message = "Hướng dẫn đặt lại mật khẩu đã được gửi đến email của bạn.";
                    ViewBag.Success = true;
                    
                    // Log: Mật khẩu hiện tại của user (chỉ để demo, production nên gửi email)
                    // ViewBag.TempPassword = user.MatKhau;
                }
                else
                {
                    ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");
                }
            }

            return View(model);
        }

        // GET: Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account_65131559");
        }

        // GET: Account_65131559/ResetPassword?token=xxx
        [AllowAnonymous]
        public ActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "Link reset mật khẩu không hợp lệ";
                return View("ResetPasswordError");
            }

            // Validate token
            var tokenInfo = PasswordResetToken.ValidateToken(token);
            if (tokenInfo == null)
            {
                ViewBag.ErrorMessage = "Link reset mật khẩu đã hết hạn hoặc không hợp lệ. Vui lòng yêu cầu reset lại.";
                return View("ResetPasswordError");
            }

            // Lấy thông tin user
            var user = db.NguoiDungs.Find(tokenInfo.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Không tìm thấy người dùng";
                return View("ResetPasswordError");
            }

            // Pass token và user info vào view
            ViewBag.Token = token;
            ViewBag.Email = user.Email;
            ViewBag.HoTen = user.HoTen;

            return View();
        }

        // POST: Account_65131559/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string token, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "Token không hợp lệ";
                return View("ResetPasswordError");
            }

            // Validate token
            var tokenInfo = PasswordResetToken.ValidateToken(token);
            if (tokenInfo == null)
            {
                ViewBag.ErrorMessage = "Link reset mật khẩu đã hết hạn hoặc không hợp lệ";
                return View("ResetPasswordError");
            }

            // Validate passwords
            if (string.IsNullOrEmpty(newPassword))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập mật khẩu mới";
                ViewBag.Token = token;
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu xác nhận không khớp";
                ViewBag.Token = token;
                return View();
            }

            if (newPassword.Length < 6)
            {
                ViewBag.ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự";
                ViewBag.Token = token;
                return View();
            }

            // Update password
            var user = db.NguoiDungs.Find(tokenInfo.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Không tìm thấy người dùng";
                return View("ResetPasswordError");
            }

            user.MatKhau = newPassword;
            db.SaveChanges();

            // Remove token after successful reset
            PasswordResetToken.RemoveToken(token);

            // Success view
            ViewBag.SuccessMessage = "Mật khẩu đã được thay đổi thành công!";
            ViewBag.HoTen = user.HoTen;
            return View("ResetPasswordSuccess");
        }

        // GET: Account_65131559/UserProfile
        [Authorize]
        public ActionResult UserProfile()
        {
            if (Session["MaNguoiDung"] == null)
            {
                return RedirectToAction("Login");
            }

            int userId = (int)Session["MaNguoiDung"];
            var user = db.NguoiDungs.Find(userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // POST: Account_65131559/UserProfile
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile(NguoiDung model, HttpPostedFileBase avatarFile)
        {
            if (Session["MaNguoiDung"] == null)
            {
                return RedirectToAction("Login");
            }

            int userId = (int)Session["MaNguoiDung"];
            var user = db.NguoiDungs.Find(userId);
            
            if (user != null)
            {
                // Chỉ cho phép update một số trường thông tin cá nhân
                user.HoTen = model.HoTen;
                user.Email = model.Email;
                user.SoDienThoai = model.SoDienThoai;
                
                // Xử lý upload ảnh đại diện
                if (avatarFile != null && avatarFile.ContentLength > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(avatarFile.FileName).ToLower();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ViewBag.ErrorMessage = "Chỉ chấp nhận file ảnh: JPG, PNG, GIF";
                        return View(user);
                    }
                    
                    // Validate file size (5MB max)
                    if (avatarFile.ContentLength > 5 * 1024 * 1024)
                    {
                        ViewBag.ErrorMessage = "Kích thước file quá lớn. Tối đa 5MB";
                        return View(user);
                    }
                    
                    try
                    {
                        // Create Avatar directory if not exists
                        string avatarDir = Server.MapPath("~/Content/Avatar");
                        if (!Directory.Exists(avatarDir))
                        {
                            Directory.CreateDirectory(avatarDir);
                        }
                        
                        // Generate unique filename
                        string fileName = "avatar_" + userId + "_" + DateTime.Now.Ticks + fileExtension;
                        string filePath = Path.Combine(avatarDir, fileName);
                        
                        // Delete old avatar if exists and is not default
                        if (!string.IsNullOrEmpty(user.AnhDaiDien))
                        {
                            string oldFilePath = Server.MapPath(user.AnhDaiDien);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                        
                        // Save new avatar
                        avatarFile.SaveAs(filePath);
                        
                        // Update database with relative path
                        user.AnhDaiDien = "~/Content/Avatar/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = "Lỗi khi upload ảnh: " + ex.Message;
                        return View(user);
                    }
                }
                
                // Không update Mật khẩu, Vai trò, Tên đăng nhập ở đây
                
                db.SaveChanges();
                
                // Update lại Session nếu cần (ví dụ tên hiển thị)
                Session["HoTen"] = user.HoTen;

                ViewBag.SuccessMessage = "Cập nhật thông tin thành công!";
                return View(user);
            }

            return View(model);
        }

        // GET: Account_65131559/ChangePassword
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: Account_65131559/ChangePassword
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ProjectCK_65131559.Models.Forms.ChangePassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (Session["MaNguoiDung"] == null)
            {
                return RedirectToAction("Login");
            }

            int userId = (int)Session["MaNguoiDung"];
            var user = db.NguoiDungs.Find(userId);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Verify Old Password
            if (user.MatKhau != model.OldPassword)
            {
                ModelState.AddModelError("OldPassword", "Mật khẩu hiện tại không đúng.");
                return View(model);
            }

            // Update Password
            user.MatKhau = model.NewPassword;
            db.SaveChanges();

            ViewBag.SuccessMessage = "Đổi mật khẩu thành công!";
            ModelState.Clear(); // Clear form so inputs are empty
            return View();
        }

        // GET: Account_65131559/LogOff
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Home_65131559");
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