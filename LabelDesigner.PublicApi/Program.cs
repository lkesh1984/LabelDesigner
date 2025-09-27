using LabelDesigner.Data.Interfaces;
using LabelDesigner.Services.Implementations;
using LabelDesigner.Services.Interface;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBarcodeService, BarcodeService>();
builder.Services.AddScoped<ISvgConverterService, SvgConverterService>();
builder.Services.AddScoped<IPdfConverterService, PdfConverterService>();

builder.Services.AddScoped<ITemplateRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<TemplateRepository>>();
    return new TemplateRepository(connectionString, logger);
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
