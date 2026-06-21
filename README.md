# ASP.NET Core Student Management System

## 📋 Overview

A modern ASP.NET Core REST API for managing students and departments with properly configured Entity Framework Core relationships and data integrity constraints. This project demonstrates enterprise-level patterns and best practices for building scalable APIs.

### Key Features
- ✅ **One-to-Many Relationships** - Properly configured Department ↔ Student relationships
- ✅ **Cascade Delete** - Automatic removal of students when department is deleted
- ✅ **Eager Loading** - Optimized queries using `.Include()` to prevent N+1 problems
- ✅ **Repository Pattern** - Clean separation of concerns with abstraction layer
- ✅ **Dependency Injection** - Loose coupling and enhanced testability
- ✅ **Entity Constraints** - Required foreign keys for data consistency
- ✅ **Explicit Configuration** - Relationships configured in `OnModelCreating()`

---

# 🛠 Technology Stack

| Component | Technology |
|-----------|-----------|
| Framework | ASP.NET Core |
| Language | C# 14.0 |
| Target | .NET 10 |
| ORM | Entity Framework Core |
| Database | SQL Server |
| IDE | Visual Studio 2026 |
| API Documentation | Swagger/OpenAPI |
| Package Manager | NuGet |

---

# 📁 Project Architecture

```
ASP.NETCORE/
├── Models/                          # Data Models
│   ├── Student.cs                   # Student entity with Department FK
│   └── Department.cs                # Department entity with navigation property
├── Data/
│   └── ApplicationDbContext.cs       # EF Core DbContext + Relationship Config
├── Repositories/                    # Data Access Layer
│   ├── IStudentRepository.cs        # Student interface
│   ├── StudentRepository.cs         # Student implementation (with eager loading)
│   ├── IDepartmentRepository.cs     # Department interface
│   └── DepartmentRepository.cs      # Department implementation
├── Controllers/                     # API Endpoints
│   ├── StudentsController.cs        # Student CRUD operations
│   └── DepartmentController.cs      # Department CRUD operations
├── Program.cs                       # Dependency Injection & Configuration
├── appsettings.json                 # Production settings
├── appsettings.Development.json     # Development settings
└── README.md                        # This file
```

---

# 📊 Database Relationship Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    ONE-TO-MANY RELATIONSHIP                 │
└─────────────────────────────────────────────────────────────┘

Department (Parent)
  ├── Id (PK)
  ├── Name
  └── Students (ICollection<Student>)
              ↑
              │
              │ (1 : Many)
              │
              ↓
         Student (Child)
           ├── Id (PK)
           ├── Name
           ├── Age
           └── DepartmentId (FK) ← Required constraint
```

### Example Data
**Department Table**
| Id | Name |
|----|------|
| 1  | IT   |
| 2  | HR   |

**Student Table**
| Id | Name     | Age | DepartmentId |
|----|----------|-----|--------------|
| 1  | Soorya   | 25  | 1            |
| 2  | Ravi     | 26  | 1            |
| 3  | Priya    | 24  | 2            |

---

# 🔧 Step-by-Step Project Setup

## Step 1: Clone the Repository

```bash
git clone https://github.com/Sooryakumar-R/ASP.NETCORE.git
cd ASP.NETCORE
```

---

## Step 2: Install Required Packages (Already Included)

```bash
# All required packages are already included in the project file
# If needed, restore with:
dotnet restore
```

**Packages Used:**
- `Microsoft.EntityFrameworkCore.SqlServer` - SQL Server provider
- `Microsoft.EntityFrameworkCore.Tools` - EF Core CLI tools

---

## Step 3: Data Models

### Department.cs
```csharp
namespace ASP.NETCORE.Models
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Student>? Students { get; set; }
            = new List<Student>();
    }
}
```

### Student.cs
```csharp
namespace ASP.NETCORE.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public int DepartmentId { get; set; }                    // ← Foreign Key

        public Department? Department { get; set; }              // ← Navigation Property
    }
}
```

---

## Step 4: Database Context Configuration

### ApplicationDbContext.cs
```csharp
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCORE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Student> Students { get; set; }
        public DbSet<Models.Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✨ KEY FIX: Explicit relationship configuration
            // Configure one-to-many relationship between Department and Student
            modelBuilder.Entity<Models.Student>()
                .HasOne(s => s.Department)                      // Student has one Department
                .WithMany(d => d.Students)                      // Department has many Students
                .HasForeignKey(s => s.DepartmentId)            // FK property
                .OnDelete(DeleteBehavior.Cascade);              // Cascade delete

            // Configure required foreign key
            modelBuilder.Entity<Models.Student>()
                .Property(s => s.DepartmentId)
                .IsRequired();                                   // DepartmentId is required
        }
    }
}
```

**Key Configuration Points:**
- ✅ `HasOne()` - Defines the one side of the relationship
- ✅ `WithMany()` - Defines the many side of the relationship
- ✅ `HasForeignKey()` - Specifies the foreign key property
- ✅ `OnDelete(DeleteBehavior.Cascade)` - Enables cascade delete
- ✅ `IsRequired()` - Makes DepartmentId mandatory

---

## Step 6: Configure Connection String

appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection":
      "Server=SOORYA-S-LPTP\\SQLEXPRESS;Database=StudentDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Update the server name according to your SQL Server instance.

---

## Step 7: Register DbContext

Program.cs

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

---

## Step 8: Create Repository Interfaces

Repositories/Interfaces

### IStudentRepository.cs

```csharp
public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();

    Task<Student?> GetByIdAsync(int id);

    Task<Student> AddAsync(Student student);

    Task UpdateAsync(Student student);

    Task DeleteAsync(Student student);
}
```

---

## Step 9: Create Repository Implementation

Repositories/StudentRepository.cs

```csharp
public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _context.Students.ToListAsync();
    }
}
```

Implement the remaining methods similarly.

---

## Step 10: Register Repository

Program.cs

```csharp
builder.Services.AddScoped<
    IStudentRepository,
    StudentRepository>();
```

---

## Step 11: Create Service Layer

Create:

Services

Services/Interfaces

Example:

```csharp
public interface IStudentService
{
    Task<List<Student>> GetAllAsync();
}
```

StudentService will call the Repository.

---

## Step 12: Create Controller

StudentsController.cs

```csharp
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(
        IStudentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students =
            await _service.GetAllAsync();

        return Ok(students);
    }
}
```

---

## Step 13: Create Migration

Open Package Manager Console

```powershell
Add-Migration InitialCreate
```

Or

```bash
dotnet ef migrations add InitialCreate
```

---

## Step 14: Update Database

```powershell
Update-Database
```

Or

```bash
dotnet ef database update
```

EF Core automatically creates:

* StudentDB
* Departments Table
* Students Table
* Foreign Key Relationship

---

## Step 15: Run Application

```bash
dotnet run
```

Or press:

```text
F5
```

Open Swagger:

```text
https://localhost:xxxx/swagger
```

---

# API Endpoints

## Departments

GET /api/departments

GET /api/departments/{id}

POST /api/departments

PUT /api/departments/{id}

DELETE /api/departments/{id}

---

## Students

GET /api/students

GET /api/students/{id}

POST /api/students

PUT /api/students/{id}

DELETE /api/students/{id}

---

# Entity Framework Core Concepts Covered

* DbContext
* DbSet
* Migrations
* LINQ
* Navigation Properties
* Foreign Keys
* Include()
* One-to-Many Relationships
* CRUD Operations

---

# 🔐 Authentication & Authorization

## Overview
The project implements **JWT (JSON Web Token)** based authentication with role-based authorization to secure API endpoints.

## Features
- ✅ **User Registration** - Create new user accounts with password hashing using BCrypt
- ✅ **User Login** - Authenticate users and issue JWT tokens
- ✅ **JWT Token Generation** - Secure tokens with 1-hour expiration
- ✅ **Role-Based Authorization** - Support for different user roles (e.g., Admin, User)
- ✅ **Token Validation** - Verify issuer, audience, signature, and expiration
- ✅ **BCrypt Password Hashing** - Secure password storage with salt

## Authentication Flow

```
┌─────────────┐         ┌──────────────┐         ┌──────────────┐
│   Client    │────────▶│  AuthController  │────────▶│  Database    │
│             │         │                 │         │              │
└─────────────┘         └──────────────────┘         └──────────────┘
     │
     │ 1. Register/Login with credentials
     │
     ├─▶ Register: Hash password + Store user
     │
     ├─▶ Login: Verify password + Generate JWT
     │
     ◀─ Receive JWT Token
     │
     │ 2. Use token in subsequent requests
     │
     ├─▶ API Request with Authorization header
     │   "Authorization: Bearer {token}"
     │
     ◀─ Access granted with user claims
```

## JWT Token Structure

```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "username",
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Admin",
    "exp": 1735689600,
    "iss": "StudentApi",
    "aud": "StudentApi"
  },
  "signature": "HMACSHA256(...)"
}
```

## Setup Instructions

### Step 1: Configure JWT Settings

**appsettings.Development.json**

```json
{
  "Jwt": {
    "Key": "MyVeryStrongSecretKey123456789ABCDEF",
    "Issuer": "StudentApi",
    "Audience": "StudentApi"
  }
}
```

**Important:** Use a strong, random key (minimum 32 characters) for production.

### Step 2: Register Authentication in Program.cs

```csharp
// JWT Authentication Configuration
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

// Enable middleware
app.UseAuthentication();
app.UseAuthorization();
```

### Step 3: Create User Model

**Models/AppUser.cs**

```csharp
namespace ASP.NETCORE.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; } = "User";  // Default role
    }
}
```

### Step 4: Add Users Table to DbContext

**Data/ApplicationDbContext.cs**

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<AppUser> Users { get; set; }  // ← Add this

    // ... existing configuration
}
```

### Step 5: Create Auth DTOs

**DTOs/AuthDtos/RegisterDto.cs**

```csharp
namespace ASP.NETCORE.DTOs.AuthDtos
{
    public class RegisterDto
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Role { get; set; } = "User";
    }
}
```

**DTOs/AuthDtos/LoginDto.cs**

```csharp
namespace ASP.NETCORE.DTOs.AuthDtos
{
    public class LoginDto
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
```

### Step 6: AuthController Implementation

**Controllers/AuthController.cs**

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterDto dto)
    {
        if (_context.Users.Any(x => x.Username == dto.Username))
            return BadRequest("User already exists");

        var user = new AppUser
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("User Registered");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var user = _context.Users.FirstOrDefault(x => x.Username == dto.Username);

        if (user == null)
            return Unauthorized();

        bool validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!validPassword)
            return Unauthorized();

        string token = GenerateJwtToken(user);

        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(AppUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### Step 7: Secure Endpoints with Authorization

Use `[Authorize]` attribute to protect routes:

```csharp
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    // Public endpoint
    [HttpGet]
    public async Task<IActionResult> GetAll() { ... }

    // Protected endpoint - requires authentication
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentDto dto) { ... }

    // Admin-only endpoint - requires Admin role
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) { ... }
}
```

## Authentication API Endpoints

### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "password": "SecurePassword123!",
  "role": "User"
}
```

**Response:**
```json
"User Registered"
```

---

### Login User
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "johndoe",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiam9obmRvZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3MzU2ODk2MDB9...."
}
```

---

### Access Protected Endpoint
```http
GET /api/students
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Security Best Practices

- ✅ **Never commit secrets** - Use User Secrets or Azure Key Vault for production
- ✅ **Use HTTPS** - Always transmit tokens over encrypted connections
- ✅ **Validate tokens** - Verify issuer, audience, and signature
- ✅ **Short expiration** - Use tokens with limited lifetime (1 hour recommended)
- ✅ **Strong passwords** - Enforce password policies during registration
- ✅ **BCrypt hashing** - Always hash passwords with salt before storage
- ✅ **CORS configuration** - Restrict cross-origin requests to trusted domains

## Required NuGet Packages

- `System.IdentityModel.Tokens.Jwt` - JWT token creation and validation
- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT bearer scheme implementation
- `BCrypt.Net-Core` - Password hashing with BCrypt
- `Microsoft.EntityFrameworkCore` - ORM for data persistence

Install with:
```bash
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package BCrypt.Net-Core
```

---

# Future Enhancements

* Refresh Token Implementation
* Two-Factor Authentication (2FA)
* OAuth2 Integration (Google, GitHub)
* Email Verification
* Pagination
* Search & Filtering
* Global Exception Handling
* Unit Testing (xUnit)
* React Frontend
* Angular Frontend
* Docker Deployment

---

# Author

Sooryakumar R

.NET Developer
