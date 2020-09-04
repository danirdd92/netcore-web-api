using System.Collections.Generic;

namespace Entities.DTOs
{
    public class CreateCompanyDto : CompanyModificationDto
    {
        public IEnumerable<CreateEmployeeDto> Employees { get; set; }
    }
}
