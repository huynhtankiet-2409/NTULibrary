using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace ProjectCK_65131559.Helpers
{
    public class EmailHelper
    {
        /// <summary>
        /// Gửi email reset mật khẩu cho người dùng
        /// </summary>
        /// <param name="toEmail">Email người nhận</param>
        /// <param name="userName">Tên người dùng</param>
        /// <param name="resetToken">Token để reset mật khẩu</param>
        /// <param name="baseUrl">Base URL của website (ví dụ: http://localhost:port)</param>
        /// <returns>True nếu gửi thành công, False nếu thất bại</returns>
        public static bool SendPasswordResetEmail(string toEmail, string userName, string resetToken, string baseUrl)
        {
            try
            {
                // Tạo link reset mật khẩu
                string resetLink = $"{baseUrl}/Account_65131559/ResetPassword?token={resetToken}";

                // Tạo nội dung email
                string subject = "Reset Mật Khẩu - Thư Viện Đại Học Nha Trang";
                string body = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                            .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px; }}
                            .button {{ display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                            .footer {{ margin-top: 20px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #666; }}
                            .warning {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 10px; margin: 15px 0; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h2>🔐 Reset Mật Khẩu</h2>
                                <p>Thư Viện Đại Học Nha Trang</p>
                            </div>
                            <div class='content'>
                                <p>Xin chào <strong>{userName}</strong>,</p>
                                
                                <p>Chúng tôi nhận được yêu cầu reset mật khẩu cho tài khoản của bạn. Vui lòng nhấn vào nút bên dưới để tạo mật khẩu mới:</p>
                                
                                <div style='text-align: center;'>
                                    <a href='{resetLink}' class='button'>Reset Mật Khẩu</a>
                                </div>
                                
                                <p>Hoặc copy link sau vào trình duyệt:</p>
                                <p style='background: #fff; padding: 10px; border: 1px solid #ddd; word-break: break-all;'>{resetLink}</p>
                                
                                <div class='warning'>
                                    <strong>⚠️ Lưu ý:</strong> Link này sẽ hết hạn sau <strong>24 giờ</strong>. Nếu bạn không yêu cầu reset mật khẩu, vui lòng bỏ qua email này.
                                </div>
                                
                                <div class='footer'>
                                    <p>Email này được gửi tự động, vui lòng không trả lời.</p>
                                    <p>© 2024 Thư Viện Đại Học Nha Trang. All rights reserved.</p>
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>
                ";

                // Lấy thông tin SMTP từ Web.config
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    // SmtpClient sẽ tự động lấy config từ Web.config
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["EmailFrom"] ?? "noreply@thuviendhnhatrang.edu.vn", "Thư Viện ĐH Nha Trang");
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    smtpClient.Send(mailMessage);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi (có thể ghi vào file log hoặc database)
                System.Diagnostics.Debug.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gửi email thông báo tài khoản bị khóa
        /// </summary>
        public static bool SendAccountLockedEmail(string toEmail, string userName, string reason = "")
        {
            try
            {
                string subject = "⚠️ Tài Khoản Của Bạn Đã Bị Khóa";
                string reasonText = string.IsNullOrEmpty(reason) ? "vi phạm quy chế thư viện" : reason;
                
                string body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #dc3545;'>⚠️ Thông Báo Khóa Tài Khoản</h2>
                            <p>Xin chào <strong>{userName}</strong>,</p>
                            <p>Tài khoản của bạn đã bị khóa do: <strong>{reasonText}</strong>.</p>
                            <p>Vui lòng liên hệ với quản trị viên để biết thêm chi tiết.</p>
                            <hr>
                            <p style='font-size: 12px; color: #666;'>Thư Viện Đại Học Nha Trang</p>
                        </div>
                    </body>
                    </html>
                ";

                using (SmtpClient smtpClient = new SmtpClient())
                {
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["EmailFrom"] ?? "noreply@thuviendhnhatrang.edu.vn", "Thư Viện ĐH Nha Trang");
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    smtpClient.Send(mailMessage);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
    }
}
