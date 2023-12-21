using Fraccionamientos_LDS.Data;
using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Repositories;
using Fraccionamientos_LDS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Nombre de tu API", Version = "v1" });

    // Comenta o elimina la siguiente línea en la configuración Swagger
    // c.MapType<User>(() => new Microsoft.OpenApi.Models.OpenApiSchema { Type = "object" });

    // ... (otras configuraciones)
});

// Setting Connection string
string connectionString = builder.Configuration.GetConnectionString("AccaConnection");

builder.Services.AddDbContext<ResidentialContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<JwtService>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuer = true,
        ValidIssuer = "Fraccionamientos_LDS_Issuer", // Cambia por el nombre que prefieras
        ValidateAudience = true,
        ValidAudience = "Fraccionamientos_LDS_Audience", // Cambia por el nombre que prefieras
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Authentication Middleware
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
