using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs
{
    public class CreateCompanyDto : CompanyModificationDto
    {
        public IEnumerable<CreateEmployeeDto> Employees { get; set; }
    }
}
