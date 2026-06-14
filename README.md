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

## Step 11: Data Transfer Objects (DTOs)

DTOs are used to transfer data between the API and clients, keeping internal models separate from API contracts.

### StudentDtos/

- `CreateStudentDto.cs` - Used for POST requests
- `UpdateStudentDto.cs` - Used for PUT requests  
- `StudentResponseDto.cs` - Used for API responses

### DepartmenDtos/

- `CreateDepartmentDto.cs` - Used for POST requests
- `DepartmentResponseDto.cs` - Used for API responses

**Benefits:**
- ✅ Hide internal implementation details
- ✅ Validate input data independently
- ✅ Control what data is exposed in API responses
- ✅ Decouple API contracts from database models

---

## Step 12: AutoMapper Configuration

AutoMapper automatically maps between entities and DTOs, eliminating manual mapping code.

### Mappings/StudentProfile.cs

```csharp
using AutoMapper;
using ASP.NETCORE.DTOs.StudentDtos;
using ASP.NETCORE.Models;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<CreateStudentDto, Student>();
        CreateMap<UpdateStudentDto, Student>();
        CreateMap<Student, StudentResponseDto>();
    }
}
```

### Mappings/DepartmentProfile.cs

```csharp
using AutoMapper;
using ASP.NETCORE.DTOs.DepartmenDtos;
using ASP.NETCORE.Models;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<CreateDepartmentDto, Department>();
        CreateMap<Department, DepartmentResponseDto>();
    }
}
```

**Program.cs Registration:**

```csharp
builder.Services.AddAutoMapper(
    typeof(StudentProfile), 
    typeof(DepartmentProfile));
```

---

## Step 13: Create Controllers

### StudentsController.cs

```csharp
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _studentRepository;

    public StudentsController(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    [HttpPost]
    public IActionResult AddStudent(CreateStudentDto studentDto)
    {
        Student student = new Student() 
        { 
            Name = studentDto.Name,
            Age = studentDto.Age,
            DepartmentId = studentDto.DepartmentId
        };
        _studentRepository.Add(student);

        return CreatedAtAction(
            nameof(GetById),
            new { id = student.Id },
            student);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var student = _studentRepository.GetById(id);
        if (student == null)
            return NotFound();

        var studentDto = new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            Age = student.Age
        };
        return Ok(studentDto);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var students = _studentRepository.GetAll();
        var studentDtos = students.Select(s => new StudentResponseDto
        {
            Id = s.Id,
            Name = s.Name,
            Age = s.Age
        }).ToList();
        return Ok(studentDtos);
    }
}
```

### DepartmentController.cs

```csharp
[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentController(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    [HttpPost]
    public IActionResult AddDepartment(CreateDepartmentDto departmentDto)
    {
        Department department = new Department() 
        { 
            Name = departmentDto.Name 
        };
        _departmentRepository.Add(department);

        return CreatedAtAction(
            nameof(GetById),
            new { id = department.Id },
            department);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var department = _departmentRepository.GetById(id);
        if (department == null)
            return NotFound();

        var departmentDto = new DepartmentResponseDto
        {
            Id = department.Id,
            Name = department.Name
        };
        return Ok(departmentDto);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var departments = _departmentRepository.GetAll();
        var departmentDtos = departments.Select(d => new DepartmentResponseDto
        {
            Id = d.Id,
            Name = d.Name
        }).ToList();
        return Ok(departmentDtos);
    }
}
```

---

## Step 14: Create Migration

Open Package Manager Console

```powershell
Add-Migration InitialCreate
```

Or

```bash
dotnet ef migrations add InitialCreate
```

---

## Step 15: Update Database

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

## Step 16: Run Application

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

# Current Implementation Status

## ✅ Completed Features

- ✅ **Repository Pattern** - Both Student and Department repositories implemented
- ✅ **Dependency Injection** - All services registered and injected via constructor
- ✅ **Entity Framework Core** - SQL Server integration with migrations
- ✅ **One-to-Many Relationships** - Department ↔ Student relationships configured
- ✅ **DTOs** - Separate DTOs for Create and Response operations
- ✅ **AutoMapper** - Automatic entity-to-DTO mapping
- ✅ **Swagger/OpenAPI** - API documentation and testing interface
- ✅ **CRUD Operations** - Full Create, Read, Update, Delete functionality

## 📋 API Endpoints

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

# Entity Framework Core & Design Patterns Covered

* DbContext
* DbSet
* Migrations
* LINQ
* Navigation Properties
* Foreign Keys
* Include() for Eager Loading
* One-to-Many Relationships
* CRUD Operations
* **Repository Pattern** - Abstraction layer for data access
* **Dependency Injection** - ASP.NET Core built-in DI container
* **Data Transfer Objects (DTOs)** - Separating API contracts from domain models
* **AutoMapper** - Object-to-object mapping
* **Cascade Delete** - Automatic cleanup of related data
* **Entity Constraints** - Data integrity through required fields

---

# Future Enhancements

* Pagination
* Search & Filtering
* Global Exception Handling
* Unit Testing (xUnit)
* Integration Testing
* JWT Authentication
* Role-Based Authorization
* Validation Attributes
* Logging (Serilog)
* Database Transactions
* API Versioning
* React Frontend
* Angular Frontend
* Docker Deployment
* CI/CD Pipeline (GitHub Actions)

---

# Author

Sooryakumar R

.NET Developer
