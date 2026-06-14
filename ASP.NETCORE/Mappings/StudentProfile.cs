using ASP.NETCORE.DTOs.StudenetDtos;
using ASP.NETCORE.Models;
using AutoMapper;

namespace ASP.NETCORE.Mappings
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<CreateStudentDto, Student>();

            CreateMap<Student, CreateStudentDto>();

            CreateMap<Student, StudentResponseDto>();
            CreateMap<StudentResponseDto, Student>();
        }
    }
}
