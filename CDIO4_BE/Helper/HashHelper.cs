using System.Security.Cryptography;
using System.Text;

namespace CDIO4_BE.Helper
{
    public static class HashHelper
    {
        /// <summary>
        /// Băm mật khẩu và trả về byte[] (dùng để so sánh với DB)
        /// </summary>
        public static byte[] HashPassword(string input)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.Unicode.GetBytes(input));
        }

        /// <summary>
        /// Băm mật khẩu và trả về hex string (dùng cho debug hoặc lưu string)
        /// </summary>
        public static string HashPasswordHex(string input)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hash); // .NET 5+ / .NET 6+
        }
    }
}
