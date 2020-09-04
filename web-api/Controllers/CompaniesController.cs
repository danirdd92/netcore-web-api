using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using web_api.ModelBinders;

namespace web_api.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repository, ILoggerManager logger,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {
            var companies = _repository.Company.GetAllCompanies(trackChanges: false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            return Ok(companiesDto);
        }

        [HttpGet("{id}", Name = "CompanyById")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
            if (company is null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                var companyDto = _mapper.Map<CompanyDto>(company);
                return Ok(companyDto);
            }
        }

        [HttpGet(Name = "CompanyCollection")]
        public IActionResult GetCompanyCollection
            ([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids is null)
            {
                _logger.LogError("Parameter \"ids\" is null");
                return BadRequest("Parameter \"ids\" is null");
            }

            var companyEntities = _repository.Company.GetCompaniesById(ids, trackChanges: false);

            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in the collection");
                return NotFound();
            }

            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }

        [HttpPost]
        public IActionResult CreateCompany([FromBody] CreateCompanyDto companyDto)
        {
            if (companyDto is null)
            {
                _logger.LogError("CreateCompanyDto object sent from client is null.");
                return BadRequest("Object is null");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the companyDto object");
                return UnprocessableEntity(ModelState);
            }

            var companyEntity = _mapper.Map<Company>(companyDto); // maps the dto into a company model

            _repository.Company.CreateCompany(companyEntity);
            _repository.Save();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity); // maps the model into a company dto

            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);
        }

        [HttpPost("collection")]
        public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CreateCompanyDto> companyCollection)
        {
            if (companyCollection is null)
            {
                _logger.LogError("Company Collection sent from client is null.");
                return BadRequest("Company collection is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for one or more objects in companyCollection");
                return UnprocessableEntity(ModelState);
            }

            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }

            _repository.Save();

            var returnCollection = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", returnCollection.Select(c => c.Id));

            return CreatedAtRoute("CompanyCollection", new { ids }, returnCollection);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCompany(Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
            if (company is null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _repository.Company.DeleteCompany(company);
            _repository.Save();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCompany(Guid id, [FromBody] UpdateCompanyDto companyDto)
        {
            if (companyDto is null)
            {
                _logger.LogError("UpdateCompanyDto object sent by client is null.");
                return BadRequest("Object is null");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the companyDto object");
                return UnprocessableEntity(ModelState);
            }

            var companyEntity = _repository.Company.GetCompany(id, trackChanges: true);
            if (companyEntity is null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            _mapper.Map(companyDto, companyEntity);
            _repository.Save();

            return NoContent();
        }
    }
}
