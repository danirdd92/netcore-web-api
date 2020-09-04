using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace web_api.ActionFilters
{
    public class ValidateEmployeeForCompanyExistsAttribute : IAsyncActionFilter
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;

        public ValidateEmployeeForCompanyExistsAttribute(ILoggerManager logger,
                                                         IRepositoryManager repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var trackChanges = (method.Equals("PUT") || method.Equals("PATCH")) 
                ? true : false;

            var companyId = (Guid)context.ActionArguments["companyId"];
            var company = await _repository.Company.GetCompanyAsync(companyId, false);

            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                context.Result = new NotFoundResult();
                return;
            }

            var id = (Guid)context.ActionArguments["id"];
            var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if (employee is null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("employee", employee);
                await next();
            }
        }
    }
}
