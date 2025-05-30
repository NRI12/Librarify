using BCrypt.Net;

namespace libraryproject.Helpers
{
    public static class PasswordHasher
    {
        // Tạo chuỗi hash từ mật khẩu
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Xác thực mật khẩu với chuỗi hash
        public static bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}