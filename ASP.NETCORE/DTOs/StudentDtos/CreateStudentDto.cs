using System.ComponentModel.DataAnnotations;

namespace ASP.NETCORE.DTOs.StudenetDtos
{
    public class CreateStudentDto
    {
        [Required]
        public string Name { get; set; }

        [Range(18, 60)]
        public int Age { get; set; }

        [Required]
        public int DepartmentId { get; set; }
    }
}
