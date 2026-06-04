# Student Management API

## Overview

Student Management API is a RESTful Web API built using ASP.NET Core, Entity Framework Core, and SQL Server.

The project demonstrates:

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* Repository Pattern
* Dependency Injection
* One-to-Many Relationships
* LINQ Queries
* CRUD Operations
* Migrations

---

# Technology Stack

* ASP.NET Core Web API
* C#
* Entity Framework Core
* SQL Server
* SQL Server Management Studio (SSMS)
* Swagger/OpenAPI

---

# Project Architecture

StudentManagementAPI

├── Controllers

├── Services

├── Repositories

├── DTOs

├── Models

├── Data

├── Migrations

├── appsettings.json

└── Program.cs

---

# Database Relationship

Department

↓

One Department can have Many Students

↓

Students

Example:

Department

| Id | Name |
| -- | ---- |
| 1  | IT   |

Students

| Id | Name   | DepartmentId |
| -- | ------ | ------------ |
| 1  | Soorya | 1            |
| 2  | Ravi   | 1            |

---

# Step-by-Step Project Setup

## Step 1: Create Project

Open Terminal or Package Manager Console

```bash
dotnet new webapi -n StudentManagementAPI
```

Open the project in Visual Studio.

---

## Step 2: Install Required Packages

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

dotnet add package Microsoft.EntityFrameworkCore.Tools
```

---

## Step 3: Create Project Folders

Create the following folders:

```text
Controllers

Services

Repositories

DTOs

Models

Data

Migrations
```

---

## Step 4: Create Models

### Department.cs

```csharp
public class Department
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<Student> Students { get; set; }
        = new List<Student>();
}
```

### Student.cs

```csharp
public class Student
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public int Age { get; set; }

    public int DepartmentId { get; set; }

    public Department Department { get; set; }
}
```

---

## Step 5: Create DbContext

Create:

Data/ApplicationDbContext.cs

```csharp
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }

    public DbSet<Department> Departments { get; set; }
}
```

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

# Future Enhancements

* DTO Mapping
* AutoMapper
* JWT Authentication
* Role-Based Authorization
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
