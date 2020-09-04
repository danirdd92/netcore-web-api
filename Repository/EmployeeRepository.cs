using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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


        public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges) =>

            FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id),
                trackChanges).SingleOrDefault();

        public IEnumerable<Employee> GetEmployees(Guid comapnyId, bool trackChanges) =>

            FindByCondition(e => e.CompanyId.Equals(comapnyId), trackChanges)
            .OrderBy(e => e.Name);
    }
}
