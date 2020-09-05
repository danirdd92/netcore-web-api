using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using web_api.ActionFilters;

namespace web_api.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IDataShaper<EmployeeDto> _dataShaper;

        public EmployeesController(IRepositoryManager repository, ILoggerManager logger,
                                    IMapper mapper, IDataShaper<EmployeeDto> dataShaper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _dataShaper = dataShaper;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
                                                                [FromQuery] EmployeeParameters employeeParameters)
        {
            if (!employeeParameters.ValidAgeRange)
            {
                return BadRequest("Minimum age can't be greater then max age.");
            }
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges:
            false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId,
            employeeParameters, trackChanges: false);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(employeesFromDb.MetaData));

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
            return Ok(_dataShaper.ShapeData(employeesDto, employeeParameters.Fields)); // Use Shape Data to constraint sent properties of the Dto
        }

        [HttpGet("{id}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);
            if (employee is null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the databse.");
                return NotFound();
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            return Ok(employeeDto);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId,
                              [FromBody] CreateEmployeeDto employeeDto)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);

            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeEntity = _mapper.Map<Employee>(employeeDto);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

            return CreatedAtRoute("GetEmployeeForCompany",
                new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            var employeeToDelete = HttpContext.Items["employee"] as Employee;

            _repository.Employee.DeleteEmployee(employeeToDelete);
            await _repository.SaveAsync();

            return NoContent();
        }


        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id,
                             [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            var employeeEntity = HttpContext.Items["employee"] as Employee;

            _mapper.Map(updateEmployeeDto, employeeEntity); // while mapping from dto -> model  changes are made and tracked by ef core
            await _repository.SaveAsync();

            return NoContent();
        }


        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]
        public async Task<IActionResult> PartiallyUpdateEmoployeeForCompany(Guid companyId, Guid id,
                                             [FromBody] JsonPatchDocument<UpdateEmployeeDto> patchDoc)
        {
            if (patchDoc is null)
            {
                _logger.LogError("PatchDoc object sent by client is null.");
                return BadRequest("Object is null.");
            }

            var employeeEntity = HttpContext.Items["employee"] as Employee;

            var empToPatch = _mapper.Map<UpdateEmployeeDto>(employeeEntity);
            patchDoc.ApplyTo(empToPatch, ModelState);

            TryValidateModel(empToPatch); // Validate the model before any changed made on the db
                                          // since we didn't map empToPatch into employeeEntity yet.

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document.");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(empToPatch, employeeEntity);
            await _repository.SaveAsync();

            return NoContent();


        }
    }
}
