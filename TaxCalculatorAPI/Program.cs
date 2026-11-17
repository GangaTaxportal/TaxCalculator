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
builder.Services.AddSingleton<TaxCalculatorAPI.Services.TaxRuleProvider>();
builder.Services.AddSingleton<TaxCalculatorAPI.Services.SlabCalculator>();
builder.Services.AddSingleton<TaxCalculatorAPI.Services.OldRegimeCalculator>();
builder.Services.AddSingleton<TaxCalculatorAPI.Services.NewRegimeCalculator>();
builder.Services.AddScoped<TaxCalculatorAPI.Services.Interfaces.ITaxComparisonService, TaxCalculatorAPI.Services.TaxComparisonService>();
builder.Services.AddScoped<TaxCalculatorAPI.Validation.CompareTaxRequestValidator>();

var app = builder.Build();

// Validate externalized tax rules before accepting requests.
_ = app.Services.GetRequiredService<TaxCalculatorAPI.Services.TaxRuleProvider>().Get("2026-27");
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("ReactFrontend");
app.UseAuthorization();

app.MapControllers();

app.Run();
