using LabelDesigner.Data.Interfaces;
using LabelDesigner.Services.Implementations;
using LabelDesigner.Services.Interface;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Listen on a specific URL
// builder.WebHost.UseUrls("http://localhost:7025", "https://localhost:55903");
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => 
{ 
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

builder.Services.AddScoped<IBarcodeService, BarcodeService>();
builder.Services.AddScoped<ISvgConverterService, SvgConverterService>();
builder.Services.AddScoped<IPdfConverterService, PdfConverterService>();

builder.Services.AddScoped<ITemplateRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("LabelDesignerDb");
    var logger = sp.GetRequiredService<ILogger<TemplateRepository>>();
    return new TemplateRepository(connectionString, logger);
});
builder.Services.AddSwaggerGen(c =>
{
    c.SupportNonNullableReferenceTypes(); // disable if enabled
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
