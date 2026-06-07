using System.ComponentModel.DataAnnotations;

namespace ASP.NETCORE.DTOs.StudenetDtos
{
    public class UpdateStudentDto
    {
        [Required]
        public string Name { get; set; }

        [Range(1, 100)]
        public int Age { get; set; }
    }
}
