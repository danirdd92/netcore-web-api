using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace web_api.ActionFilters
{
    public class ValidateCompanyExistsAttribute : IAsyncActionFilter
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;

        public ValidateCompanyExistsAttribute(ILoggerManager logger,
                                              IRepositoryManager repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var trackChanges = context.HttpContext.Request.Method.Equals("PUT"); // only track on PUT requests
            var id = (Guid)context.ActionArguments["id"];
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges);

            if (company is null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("company", company);
                await next();
            }
        }
    }
}
