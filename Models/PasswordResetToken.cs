using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectCK_65131559.Models
{
    /// <summary>
    /// Model để lưu trữ token reset mật khẩu tạm thời
    /// Sử dụng static dictionary để lưu trong bộ nhớ (cho đơn giản)
    /// Trong production nên lưu vào database
    /// </summary>
    public class PasswordResetToken
    {
        private static Dictionary<string, TokenInfo> _tokens = new Dictionary<string, TokenInfo>();

        public class TokenInfo
        {
            public int UserId { get; set; }
            public string Email { get; set; }
            public DateTime ExpiryTime { get; set; }
        }

        /// <summary>
        /// Tạo token mới cho user
        /// </summary>
        public static string GenerateToken(int userId, string email)
        {
            // Tạo token ngẫu nhiên (GUID)
            string token = Guid.NewGuid().ToString("N");

            // Lưu token với thời gian hết hạn 24 giờ
            _tokens[token] = new TokenInfo
            {
                UserId = userId,
                Email = email,
                ExpiryTime = DateTime.Now.AddHours(24)
            };

            // Xóa các token đã hết hạn
            CleanExpiredTokens();

            return token;
        }

        /// <summary>
        /// Kiểm tra token có hợp lệ không
        /// </summary>
        public static TokenInfo ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            if (_tokens.ContainsKey(token))
            {
                TokenInfo info = _tokens[token];
                
                // Kiểm tra token có hết hạn chưa
                if (info.ExpiryTime > DateTime.Now)
                {
                    return info;
                }
                else
                {
                    // Token hết hạn, xóa đi
                    _tokens.Remove(token);
                }
            }

            return null;
        }

        /// <summary>
        /// Xóa token sau khi đã sử dụng
        /// </summary>
        public static void RemoveToken(string token)
        {
            if (_tokens.ContainsKey(token))
            {
                _tokens.Remove(token);
            }
        }

        /// <summary>
        /// Xóa các token đã hết hạn
        /// </summary>
        private static void CleanExpiredTokens()
        {
            var expiredTokens = _tokens.Where(t => t.Value.ExpiryTime < DateTime.Now)
                                       .Select(t => t.Key)
                                       .ToList();

            foreach (var token in expiredTokens)
            {
                _tokens.Remove(token);
            }
        }

        /// <summary>
        /// Xóa tất cả token của một user (ví dụ khi user đã reset password thành công)
        /// </summary>
        public static void RemoveUserTokens(int userId)
        {
            var userTokens = _tokens.Where(t => t.Value.UserId == userId)
                                    .Select(t => t.Key)
                                    .ToList();

            foreach (var token in userTokens)
            {
                _tokens.Remove(token);
            }
        }
    }
}
