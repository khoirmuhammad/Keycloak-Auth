var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost:8080/realms/employeeapi";
                    options.Audience = "employeeapi-api";
                    options.RequireHttpsMetadata = false;
                });

builder.Services.AddAuthorizationBuilder();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var employees = new List<Employee>
{
    new Employee
    {
        Id = 1,
        FirstName = "Naskala",
        LastName = "Beryl",
        Age = 20,
        Salary = 5000,
        Currency = "USD",
        Address = "New York"
    },
    new Employee
    {
        Id = 2,
        FirstName = "Muhammad",
        LastName = "Khoirudin",
        Age = 30,
        Salary = 4500,
        Currency = "EUR",
        Address = "Berlin"
    },
    new Employee
    {
        Id = 3,
        FirstName = "Dea",
        LastName = "Alvine",
        Age = 29,
        Salary = 18000000,
        Currency = "IDR",
        Address = "Jakarta"
    }
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

/// GET ALL (only firstname, lastname, address)
app.MapGet("/employees", () =>
{
    return employees.Select(e => new
    {
        e.FirstName,
        e.LastName,
        e.Address
    });
}).WithOpenApi();

/// GET BY ID (all data)
app.MapGet("/employees/{id:int}", (int id) =>
{
    var employee = employees.FirstOrDefault(e => e.Id == id);

    if (employee == null)
        return Results.NotFound("Employee not found");

    return Results.Ok(employee);
}).WithOpenApi().RequireAuthorization();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

internal record Employee
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public int Age { get; init; }
    public decimal Salary { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}
