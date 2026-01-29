using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Employees
{
    public class CreateEmployeeModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateEmployeeModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public EmployeeCreateDto Employee { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("EmployeeApi");

            var response = await client.PostAsJsonAsync("/api/employees", Employee);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            return Page();
        }

        public class EmployeeCreateDto
        {
            public string Name { get; set; } = "";
            public string Department { get; set; } = "";
            public decimal Salary { get; set; }
        }
    }
}
