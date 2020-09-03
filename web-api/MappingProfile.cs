﻿using AutoMapper;
using Entities.DTOs;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            CreateMap<UpdateCompanyDto, Company>();
        }
    }
}
