using Microsoft.AspNetCore.Authentication.JwtBearer;
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
// Thêm tất cả các service đã đăng ký trong hai đoạn code
builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
builder.Services.AddScoped<ITacPhamService, TacPhamService>();
builder.Services.AddScoped<IAdminService, AdminService>();
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

    // Tùy chỉnh xử lý lỗi 401
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                title = "Unauthorized",
                status = 401,
                detail = "Token không hợp lệ hoặc đã hết hạn",
                traceId = context.HttpContext.TraceIdentifier
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();