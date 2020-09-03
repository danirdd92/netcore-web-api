using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface ICompanyRepository
    {
        IEnumerable<Company> GetAllCompanies(bool trackChanges);
        Company GetCompany(Guid companyId, bool trackChanges);
        IEnumerable<Company> GetCompaniesById(IEnumerable<Guid> ids, bool trackChanges);
        void CreateCompany(Company company);
    }
}
