using DotnetApi.Data.Entities;
using DotnetApi.Features.Employees.Models;

namespace DotnetApi.Features.Employees.Mapping
{
    public static class EmployeeMappings
    {
        public static EmployeeDto ToDto(this Employee e) =>
       new(e.Id, e.Name, e.Department, e.Salary, e.CreatedAt);

        public static Employee ToEntity(this EmployeeDto dto) =>
       new()
       {
           Name = dto.Name,
           Department = dto.Department,
           Salary = dto.Salary
       };
    }
}
