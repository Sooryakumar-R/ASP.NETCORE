using Microsoft.AspNetCore.Builder;
using ASP.NETCORE.Data;
using Microsoft.EntityFrameworkCore;
using ASP.NETCORE.Repositories;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ASP.NETCORE.Repositories.IStudentRepository, ASP.NETCORE.Repositories.StudentRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

// Swagger middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();