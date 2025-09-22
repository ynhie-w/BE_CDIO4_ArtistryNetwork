using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CDIO4_BE.Helper
{
    public static class JwtHelper
    {
        // Khóa bí mật ≥ 32 ký tự (256 bit)
        private static string KhoaBiMat = "SuperSecretKey123!ChangeThisToLongerKey456!";

        public static string TaoToken(int IdNguoiDung, string VaiTro, int ThoiGianHieuLuc = 60)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, IdNguoiDung.ToString()),
                   new Claim(ClaimTypes.NameIdentifier, IdNguoiDung.ToString()),
                new Claim(ClaimTypes.Role, VaiTro)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KhoaBiMat));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "AppCuaBan",
                audience: "AppCuaBan",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(ThoiGianHieuLuc),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
