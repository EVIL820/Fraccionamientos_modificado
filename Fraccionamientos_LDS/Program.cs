using Fraccionamientos_LDS.Data;
using Fraccionamientos_LDS.Entities;
using Fraccionamientos_LDS.Repositories;
using Fraccionamientos_LDS.Repositories.Interfaces;
using Fraccionamientos_LDS.Services;
using Fraccionamientos_LDS.Services.Interfaces; // Agregado
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
});

// Setting Connection string
string connectionString = builder.Configuration.GetConnectionString("AccaConnection");

builder.Services.AddDbContext<ResidentialContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>(); // Cambiado a IJwtService

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
        ValidIssuer = jwtSettings.Issuer, // Utiliza la configuraci�n
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience, // Utiliza la configuraci�n
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
