using System;

/// <summary>
/// Dto objects do not contain EF related markup
/// </summary>
namespace Entities.DTOs
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FullAddress { get; set; }
    }
}
