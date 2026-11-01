
using Microsoft.EntityFrameworkCore; 
using PortfolioApi.Data; 
using PortfolioApi.Services; 

// Creates the builder used to configure the web application.
var builder = WebApplication.CreateBuilder(args);

// Enables the use of API controllers.
builder.Services.AddControllers();

// Registers ProjectService so it can be injected through IProjectService.
builder.Services.AddScoped<IProjectService, ProjectService>();

// Registers BlogpostService so it can be injected through IBlogpostService.
builder.Services.AddScoped<IBlogpostService, BlogpostService>();

// Registers PortfolioDbContext for database access.
builder.Services.AddDbContext<PortfolioDbContext>(options =>
    // Configures Entity Framework Core to use SQL Server.
    options.UseSqlServer(
        // Reads the database connection string from configuration.
        builder.Configuration.GetConnectionString("PortfolioDb")));

// Registers Swagger to generate API documentation.
builder.Services.AddSwaggerGen();

// Registers the built-in OpenAPI document generation service.
builder.Services.AddOpenApi();

// Builds the application using the configured services.
var app = builder.Build();

// Checks whether the application is running in Development mode.
if (app.Environment.IsDevelopment())
{
    // Makes the built-in OpenAPI document available.
    app.MapOpenApi();

    // Enables the Swagger documentation endpoint.
    app.UseSwagger();

    // Enables the Swagger UI for testing API endpoints.
    app.UseSwaggerUI();
}

// Redirects HTTP requests to HTTPS when HTTPS is configured.
app.UseHttpsRedirection();

// Adds the authorization middleware.
app.UseAuthorization();

// Connects the controller routes to the application.
app.MapControllers();

// Starts the application and listens for HTTP requests.
app.Run();
