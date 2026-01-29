using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Employees
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public List<EmployeeDto> Employees { get; set; } = new();


        public async Task OnGet()
        {
            var client = _httpClientFactory.CreateClient("EmployeeApi");

            var result = await client.GetFromJsonAsync<List<EmployeeDto>>("/api/employees");
            Employees = result ?? new List<EmployeeDto>
            {
                new EmployeeDto
                (
                 1,
                "boþ dto",
                "boþ veri",
                 10,
                 DateTime.UtcNow
                )
            };
        }
        public record EmployeeDto(int Id, string Name, string Department, decimal Salary, DateTime CreatedAt);
    }
}
