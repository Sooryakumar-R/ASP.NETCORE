using Microsoft.AspNetCore.Builder;
using ASP.NETCORE.Data;
using Microsoft.EntityFrameworkCore;
using ASP.NETCORE.Repositories;
using ASP.NETCORE.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Dependency Injection
builder.Services.AddScoped<ASP.NETCORE.Repositories.IStudentRepository, ASP.NETCORE.Repositories.StudentRepository>();
builder.Services.AddScoped<ASP.NETCORE.Repositories.IDepartmentRepository, ASP.NETCORE.Repositories.DepartmentRepository>();

//DBcontext - EFCore
builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

//AutoMapper - register both profiles in a single call (avoids overload confusion)
builder.Services.AddAutoMapper(typeof(StudentProfile), typeof(DepartmentProfile));

//JWT Authentication

/*It configures JWT token validation in ASP.NET Core. It verifies the token issuer, audience, expiration time, and digital signature using the configured secret key before authenticating the user.*/
builder.Services
.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer =
                builder.Configuration["Jwt:Issuer"],

            ValidAudience =
                builder.Configuration["Jwt:Audience"],

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        builder.Configuration["Jwt:Key"]!))
        };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Swagger middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();