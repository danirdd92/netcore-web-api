﻿using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {

        public EmployeeRepository(RepositoryContext context) : base(context)
        {

        }

        public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges) =>

            FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id),
                trackChanges).SingleOrDefault();

        public IEnumerable<Employee> GetEmployees(Guid comapnyId, bool trackChanges) =>

            FindByCondition(e => e.CompanyId.Equals(comapnyId), trackChanges)
            .OrderBy(e => e.Name);
    }
}
