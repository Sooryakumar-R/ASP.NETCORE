using ASP.NETCORE.Models;
using ASP.NETCORE.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        public IActionResult AddDepartment(Department department)
        {
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

            return Ok(department);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var departments = _departmentRepository.GetAll();
            return Ok(departments);
        }
    }
}
