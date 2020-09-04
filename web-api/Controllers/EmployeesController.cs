using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace web_api.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeesController(IRepositoryManager repository, ILoggerManager logger,
                                    IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetEmployeesForCompany(Guid companyId)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);

            if (company is null)
            {
                _logger.LogInfo($"Company with Id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employees = _repository.Employee.GetEmployees(companyId, trackChanges: false);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Ok(employeesDto);
        }

        [HttpGet("{id}", Name = "GetEmployeeForCompany")]
        public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employee = _repository.Employee.GetEmployee(companyId, id, trackChanges: false);
            if (employee is null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the databse.");
                return NotFound();
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            return Ok(employeeDto);
        }

        [HttpPost]
        public IActionResult CreateEmployeeForCompany(Guid companyId,
                              [FromBody] CreateEmployeeDto employeeDto)
        {
            if (employeeDto is null)
            {
                _logger.LogError("CreateEmployeeDto object sent from client is null.");
                return BadRequest("Object is null");
            }

            var company = _repository.Company.GetCompany(companyId, trackChanges: false);

            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeEntity = _mapper.Map<Employee>(employeeDto);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            _repository.Save();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

            return CreatedAtRoute("GetEmployeeForCompany",
                new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database");
                return NotFound();
            }

            var employeeToDelete = _repository.Employee.GetEmployee(companyId, id, trackChanges: false);
            if (employeeToDelete is null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database");
                return NotFound();
            }

            _repository.Employee.DeleteEmployee(employeeToDelete);
            _repository.Save();

            return NoContent();
        }


        [HttpPut("{id}")]
        public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid id,
                             [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            if (updateEmployeeDto is null)
            {
                _logger.LogError("UpdatEmployeeDto object sent by client is null.");
                return BadRequest("Object is null.");
            }

            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeEntity = _repository.Employee.GetEmployee(companyId, id, trackChanges: true); // <- Mind tracking is set to true
            if (employeeEntity is null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _mapper.Map(updateEmployeeDto, employeeEntity); // while mapping from dto -> model  changes are made and tracked by ef core
            _repository.Save();

            return NoContent();
        }
    }
}
