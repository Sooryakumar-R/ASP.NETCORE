using ASP.NETCORE.DTOs;
using ASP.NETCORE.DTOs.StudenetDtos;
using ASP.NETCORE.Models;
using ASP.NETCORE.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ASP.NETCORE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _repository;
        private readonly IMapper  _mapper;

        public StudentsController(
            IStudentRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var students = _repository.GetAll();

            //Automapper
            var dtos = _mapper.Map<List<StudentResponseDto>>(students);

            /*
            DTO:
            var dtos = students.Select(s => new StudentResponseDto()
            {
                Id = s.Id,
                Name = s.Name,
                DepartmentName = s.Department?.Name ?? string.Empty
            }).ToList();
            */
            
            return Ok(dtos);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var student = _repository.GetById(id);

            if (student == null)
                return NotFound();
            
            var dto = _mapper.Map<StudentResponseDto>(student);
           /*
             var dto = new StudentResponseDto()
            {
                Id = student.Id,
                Name = student.Name,
                DepartmentName = student.Department?.Name ?? string.Empty
            };
            */
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult AddStudent(CreateStudentDto dto)
        {
            var student = _mapper.Map<Student>(dto);

            _repository.Add(student);

            return CreatedAtAction(
                nameof(GetById),
                new { id = student.Id },
                student);
        }
    }
}
