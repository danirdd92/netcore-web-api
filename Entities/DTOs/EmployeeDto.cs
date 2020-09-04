using System;


/// <summary>
/// Dto objects do not contain EF related markup
/// </summary>
namespace Entities.DTOs
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Position { get; set; }
    }
}
