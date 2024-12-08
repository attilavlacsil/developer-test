using Taxually.TechnicalTest.Clients;
using Taxually.TechnicalTest.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddVatRegistrationProcessing(builder.Configuration.GetRequiredSection("VatRegistrationProcessing"));

builder.Services.AddScoped<TaxuallyHttpClient>();
builder.Services.AddScoped<TaxuallyQueueClient>();

var assembly = typeof(Program).Assembly;
builder.Services.AddAutoMapper(assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
