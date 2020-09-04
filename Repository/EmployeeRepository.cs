using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {

        public EmployeeRepository(RepositoryContext context) : base(context)
        {

        }

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee) => Delete(employee);


        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>

            await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id),
                trackChanges).SingleOrDefaultAsync();

        public async Task<IEnumerable<Employee>> GetEmployees(Guid comapnyId, bool trackChanges) =>

            await FindByCondition(e => e.CompanyId.Equals(comapnyId), trackChanges)
            .OrderBy(e => e.Name).ToListAsync();
    }
}
