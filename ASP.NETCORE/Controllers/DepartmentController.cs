using ASP.NETCORE.DTOs.DepartmenDtos;
using ASP.NETCORE.Models;
using ASP.NETCORE.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Linq;

namespace ASP.NETCORE.Controllers
{
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
            Department department = new Department() { Name = departmentDto.Name };
            _departmentRepository.Add(department);

            return CreatedAtAction(
                nameof(GetById),
                new { id = department.Id },
                department);
            //201 Created
            //The created object
            //A Location header pointing to the GET endpoint
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
}
