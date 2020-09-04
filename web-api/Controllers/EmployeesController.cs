﻿using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);

            if (company is null)
            {
                _logger.LogInfo($"Company with Id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employees = _repository.Employee.GetEmployeesAsync(companyId, trackChanges: false);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Ok(employeesDto);
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
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId,
                              [FromBody] CreateEmployeeDto employeeDto)
        {
            if (employeeDto is null)
            {
                _logger.LogError("CreateEmployeeDto object sent from client is null.");
                return BadRequest("Object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the CreateEmployeeDto object.");
                return UnprocessableEntity(ModelState); // Returns 422 instead of 400
            }

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
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database");
                return NotFound();
            }

            var employeeToDelete = await  _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);
            if (employeeToDelete is null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database");
                return NotFound();
            }

            _repository.Employee.DeleteEmployee(employeeToDelete);
            await  _repository.SaveAsync();

            return NoContent();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id,
                             [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            if (updateEmployeeDto is null)
            {
                _logger.LogError("UpdatEmployeeDto object sent by client is null.");
                return BadRequest("Object is null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the updateEmployeeDto object");
                return UnprocessableEntity(ModelState);
            }

            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: true); // <- Mind tracking is set to true
            if (employeeEntity is null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _mapper.Map(updateEmployeeDto, employeeEntity); // while mapping from dto -> model  changes are made and tracked by ef core
            await  _repository.SaveAsync();

            return NoContent();
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateEmoployeeForCompany(Guid companyId, Guid id,
                                             [FromBody] JsonPatchDocument<UpdateEmployeeDto> patchDoc)
        {
            if (patchDoc is null)
            {
                _logger.LogError("PatchDoc object sent by client is null.");
                return BadRequest("Object is null.");
            }

            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: true);
            if (employeeEntity is null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }

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
            await  _repository.SaveAsync();

            return NoContent();


        }
    }
}
