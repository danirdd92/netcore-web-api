using Contracts;
using Entities;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _context;
        private ICompanyRepository _companyRepository;
        private IEmployeeRepository _employeeRepository;

        public RepositoryManager(RepositoryContext context)
        {
            _context = context;
        }
        public ICompanyRepository Company
        {
            get
            {
                if (_companyRepository is null)
                    _companyRepository = new CompanyRepository(_context);
                return _companyRepository;
            }
        }

        public IEmployeeRepository Employee
        {
            get
            {
                if (_employeeRepository is null)
                    _employeeRepository = new EmployeeRepository(_context);
                return _employeeRepository;
            }
        }

        public Task SaveAsync() => _context.SaveChangesAsync();
    }
}
