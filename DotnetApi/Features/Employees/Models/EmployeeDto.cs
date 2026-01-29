namespace DotnetApi.Features.Employees.Models
{
    public record EmployeeDto(
        int Id,
        string Name,
        string Department,
        decimal Salary,
        DateTime CreatedAt
    );
}
