using DotnetApi.Data;
using DotnetApi.Features.Employees.Endpoints;
using DotnetApi.Features.Health;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (args.Contains("--migrate"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();

    Console.WriteLine(" Database migrated successfully.");
    return; // app.Run() çalýþmasýn
}
app.MapGetEmployees();
app.MapHealthEndpoints();
app.MapCreateEmployee();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
