using CODE_CDIO4.Repository;
using CODE_CDIO4.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ==================== CẤU HÌNH JWT ====================
var secretKey = builder.Configuration["Jwt:Secret"] ?? "yourSecretKey12345";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// ==================== DI SERVICES ====================
builder.Services.AddScoped<NotificationService>();

// ==================== DATABASE ====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==================== CONTROLLERS + JSON OPTIONS ====================
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    );

// ==================== SWAGGER ====================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Artistry Network API",
        Description = "API quản lý người dùng, tác phẩm, bình luận"
    });

    // ⚡️ JWT Authentication trong Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token",
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });

    c.EnableAnnotations(); // [SwaggerOperation], [SwaggerResponse]
});

// ==================== BUILD APP ====================
var app = builder.Build();

// ==================== PIPELINE ====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Artistry Network API v1");
        c.RoutePrefix = string.Empty; // mở swagger tại "/"
    });
}

app.UseHttpsRedirection();

// ⚠️ Authentication phải trước Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
