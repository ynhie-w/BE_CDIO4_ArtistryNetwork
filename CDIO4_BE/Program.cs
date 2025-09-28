﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using CDIO4_BE.Services;
using CDIO4_BE.Services.Interfaces;
using CDIO4_BE.Repository;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Logging để dễ dàng debug hơn
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ===== Đăng ký DbContext =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ===== Đăng ký các service =====
builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
builder.Services.AddScoped<ITacPhamService, TacPhamService>();
builder.Services.AddScoped<IQuanTriVienService, QuanTriVienService>();
builder.Services.AddScoped<IDonHangService, DonHangService>();
builder.Services.AddScoped<IGioHangService, GioHangService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ===== Cấu hình Swagger với JWT =====
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CDIO4_BE",
        Version = "v1"
    });

    // Thêm JWT Bearer vào Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Nhập JWT Bearer token theo format: Bearer {token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ===== Cấu hình CORS =====
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000") // Cho phép FE React gọi API
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// ===== Cấu hình JWT Authentication =====
var key = Encoding.UTF8.GetBytes("SuperSecretKey123!ChangeThisToLongerKey456!");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "AppCuaBan",
        ValidAudience = "AppCuaBan",
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                ThongBao = "Bạn chưa đăng nhập",
                KetQua = "Token không hợp lệ hoặc đã hết hạn"                
            });

            return context.Response.WriteAsync(result);
        }
    };
});

var app = builder.Build();

// ===== Middleware =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CDIO4_BE v1");
    });
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins); // ✅ Thêm CORS vào pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
