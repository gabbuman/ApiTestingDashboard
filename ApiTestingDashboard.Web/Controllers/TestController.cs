using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiTestingDashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiTestingDashboard.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            try
            {
                // Test database connection
                var canConnect = await _context.Database.CanConnectAsync();
                
                return Ok(new
                {
                    Status = "Healthy",
                    DatabaseConnected = canConnect,
                    Timestamp = DateTime.UtcNow,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("tables")]
        public async Task<IActionResult> GetTableInfo()
        {
            try
            {
                var teamCount = await _context.Teams.CountAsync();
                var collectionCount = await _context.Collections.CountAsync();
                var endpointCount = await _context.Endpoints.CountAsync();

                return Ok(new
                {
                    Tables = new
                    {
                        Teams = teamCount,
                        Collections = collectionCount,
                        Endpoints = endpointCount
                    },
                    Message = "Database tables are accessible!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message
                });
            }
        }

        [HttpGet("user-info")]
        [Authorize] // This endpoint requires authentication
        public IActionResult GetUserInfo()
        {
            var user = HttpContext.User;
            
            return Ok(new
            {
                IsAuthenticated = user.Identity?.IsAuthenticated ?? false,
                Username = user.Identity?.Name,
                UserId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                Claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList()
            });
        }
    }
}