var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<TaxCalculatorAPI.Services.Interfaces.ITaxComparisonService, TaxCalculatorAPI.Services.TaxCalculatorService>();
builder.Services.AddScoped<TaxCalculatorAPI.Validation.CompareTaxRequestValidator>();

var app = builder.Build();

// Resolve the service before serving requests so the configured rules are checked at startup.
using (var scope = app.Services.CreateScope())
    _ = scope.ServiceProvider.GetRequiredService<TaxCalculatorAPI.Services.Interfaces.ITaxComparisonService>();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("ReactFrontend");
app.UseAuthorization();

app.MapControllers();

app.Run();
