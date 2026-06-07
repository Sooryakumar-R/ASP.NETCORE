using System.ComponentModel.DataAnnotations;

namespace ASP.NETCORE.DTOs.DepartmenDtos
{
    public class CreateDepartmentDto
    {
        [Required]
        public string Name { get; set; }
    }
}
