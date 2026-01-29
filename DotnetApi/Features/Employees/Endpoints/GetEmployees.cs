using DotnetApi.Data;
using DotnetApi.Features.Employees.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Features.Employees.Endpoints
{
    public static class GetEmployeesEndpoint
    {
        public static void MapGetEmployees(this IEndpointRouteBuilder app)
        {

            app.MapGet("/api/employees", async (AppDbContext db) =>
            {
                var employees = await db.Employees
                    .Select(e => e.ToDto())
                    .ToListAsync();

                return Results.Ok(employees);
            });
        }
    }
}
