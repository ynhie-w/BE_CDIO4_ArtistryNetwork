using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CDIO4_BE.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CDIO4_BE.Helper
{
    public static class JwtHelper
    {
        // Khóa bí mật ≥ 32 ký tự (256 bit)
        private static string KhoaBiMat = "SuperSecretKey123!ChangeThisToLongerKey456!";

        public static string TaoToken(
        NguoiDung user,
        string VaiTro,
        int ThoiGianHieuLuc = 60)
        {
            var claims = new List<Claim>
    {
        new Claim("userId", user.Id.ToString()),
        new Claim("userName", user.Ten),
        new Claim("role", VaiTro),
        new Claim("email", user.Email ?? ""),
        new Claim("phone", user.Sdt ?? ""),                
        new Claim("status", user.TrangThai.ToString()),     
        new Claim("createdAt", user.NgayTao.ToString("o"))  
    };

            if (!string.IsNullOrEmpty(user.AnhDaiDien))
            {
                claims.Add(new Claim("avatar", user.AnhDaiDien));
            }

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
