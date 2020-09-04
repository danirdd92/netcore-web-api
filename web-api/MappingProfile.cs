using AutoMapper;
using Entities.DTOs;
using Entities.Models;

namespace web_api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(c => c.FullAddress, options =>
                options.MapFrom(x => string.Join(' ', x.Address, x.Country)));


            // Source <--> Destination switched for none GET requests
            CreateMap<Employee, EmployeeDto>();

            CreateMap<CreateCompanyDto, Company>();

            CreateMap<CreateEmployeeDto, Employee>();

            CreateMap<UpdateEmployeeDto, Employee>();
            CreateMap<UpdateEmployeeDto, Employee>().ReverseMap(); // Allow two-way mapping

            CreateMap<UpdateCompanyDto, Company>();


        }
    }
}
