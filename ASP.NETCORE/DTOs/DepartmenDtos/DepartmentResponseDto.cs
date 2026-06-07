using ASP.NETCORE.DTOs.StudenetDtos;
using ASP.NETCORE.Models;

namespace ASP.NETCORE.DTOs.DepartmenDtos
{
    public class DepartmentResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public List<StudentResponseDto> Student { get; set; }
    }
}
