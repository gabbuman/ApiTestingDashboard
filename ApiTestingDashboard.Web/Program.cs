// ApiTestingDashboard.Web/Program.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApiTestingDashboard.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
await ConfigureMiddleware(app);

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Database configuration
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
            npgsqlOptions => 
            {
                npgsqlOptions.MigrationsAssembly("ApiTestingDashboard.Infrastructure");
                npgsqlOptions.CommandTimeout(120); // 2 minutes timeout
            }));

    // Identity configuration
    services.AddDefaultIdentity<IdentityUser>(options => 
    {
        // Password requirements (you can adjust these)
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        
        // Email confirmation (disable for development)
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

    // Add Razor Pages for Identity
    services.AddRazorPages();
    
    // Add Blazor Server services
    services.AddServerSideBlazor();

    // Add API controllers (for our Web API endpoints)
    services.AddControllers();

    // Add SignalR for real-time features
    services.AddSignalR();

    // Add HttpClient for making API requests to external APIs
    services.AddHttpClient();

    // Add CORS for API access (useful for development)
    services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
}

static async Task ConfigureMiddleware(WebApplication app)
{
    // Configure the HTTP request pipeline
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseCors();

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Map routes
    app.MapRazorPages();
    app.MapBlazorHub();
    app.MapControllers(); // For our API endpoints
    
    // Map Blazor components
    app.MapFallbackToPage("/_Host");

    // Ensure database is created and updated
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        await context.Database.MigrateAsync();
        Console.WriteLine("✅ Database migration completed successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database migration failed: {ex.Message}");
        throw;
    }
}