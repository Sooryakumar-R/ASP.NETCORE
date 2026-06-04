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
            return Ok(_repository.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var student = _repository.GetById(id);

            if (student == null)
                return NotFound();

            return Ok(student);
        }
    }
}
