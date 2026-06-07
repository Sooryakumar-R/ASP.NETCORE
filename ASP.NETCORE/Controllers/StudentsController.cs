using ASP.NETCORE.DTOs;
using ASP.NETCORE.DTOs.StudenetDtos;
using ASP.NETCORE.Models;
using ASP.NETCORE.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NETCORE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _repository;

        public StudentsController(
            IStudentRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var students = _repository.GetAll();
            var dtos = students.Select(s => new StudentResponseDto()
            {
                Id = s.Id,
                Name = s.Name,
                DepartmentName = s.Department?.Name ?? string.Empty
            }).ToList();
            
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var student = _repository.GetById(id);

            if (student == null)
                return NotFound();
            
            var dto = new StudentResponseDto()
            {
                Id = student.Id,
                Name = student.Name,
                DepartmentName = student.Department?.Name ?? string.Empty
            };
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult AddStudent(CreateStudentDto dto)
        {
            var student = new Student
            {
                Name = dto.Name ?? string.Empty,
                DepartmentId = dto.DepartmentId
            };

            _repository.Add(student);

            return CreatedAtAction(
                nameof(GetById),
                new { id = student.Id },
                student);
        }
    }
}
