using DotnetApi.Data;
using DotnetApi.Features.Employees.Mapping;
using DotnetApi.Features.Employees.Models;

namespace DotnetApi.Features.Employees.Endpoints
{
    public static class CreateEmployeeEndpoint
    {
        public static void MapCreateEmployee(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/employees", async (EmployeeDto dto, AppDbContext db) =>
            {
                var employee = dto.ToEntity();

                db.Employees.Add(employee);
                await db.SaveChangesAsync();

                return Results.Created($"/api/employees/{employee.Id}", employee.ToDto());
            });
        }
    }
}
