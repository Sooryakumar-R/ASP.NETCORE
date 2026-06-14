using ASP.NETCORE.DTOs.DepartmenDtos;
using ASP.NETCORE.Models;
using AutoMapper;

namespace ASP.NETCORE.Mappings
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<DepartmentResponseDto, Department>();
            CreateMap<Department, DepartmentResponseDto>();


            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<Department, CreateDepartmentDto>();

        }
    }
}
