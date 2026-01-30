using DotnetApi.Data;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Features.Health
{
    public static class HealthEndpoints
    {
        public static void MapHealthEndpoints(this IEndpointRouteBuilder app)
        {
            // Liveness → Uygulama ayakta mı kontrolü
            app.MapGet("/health/live", () =>
            {
                return Results.Ok(new { status = "live" });
            });

            // Readiness → DB baglanti kontrolu
            app.MapGet("/health/ready", async (AppDbContext db) =>
            {
                try
                {
                    // Basit DB erişim testi
                    await db.Database.ExecuteSqlRawAsync("SELECT 1");
                    return Results.Ok(new { status = "ready" });
                }
                catch
                {
                    return Results.Problem("Database not ready", statusCode: 503);
                }
            });
        }
    }
}
