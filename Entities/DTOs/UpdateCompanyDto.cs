using System.Collections.Generic;

namespace Entities.DTOs
{
    public class UpdateCompanyDto : CompanyModificationDto
    {
        public IEnumerable<CreateEmployeeDto> Employees { get; set; }
    }
}
