using Microsoft.AspNetCore.Builder;
using ASP.NETCORE.Data;
using Microsoft.EntityFrameworkCore;
using ASP.NETCORE.Repositories;
using ASP.NETCORE.Mappings;
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

var app = builder.Build();

// Swagger middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();