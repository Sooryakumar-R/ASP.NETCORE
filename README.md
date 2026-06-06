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
