using System.Text;
using System.Security.Cryptography;

public static class MatKhauHelper
{
    // Hash mật khẩu giống SQL Server HASHBYTES(N'SHA2_256', NVARCHAR)
    public static byte[] HashTheoSQL(string matKhau)
    {
        // SQL Server dùng UTF-16LE để lưu NVARCHAR
        byte[] bytes = Encoding.Unicode.GetBytes(matKhau);
        using (var sha = SHA256.Create())
        {
            return sha.ComputeHash(bytes);
        }
    }

    public static bool KiemTraMatKhau(string matKhau, byte[] hash)
    {
        var hashedInput = HashTheoSQL(matKhau);
        return hashedInput.SequenceEqual(hash);
    }
}
