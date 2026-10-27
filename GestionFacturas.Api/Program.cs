using GestionFacturas.Application.Interfaces;
using GestionFacturas.Application.Services;
using GestionFacturas.Infrastructure.Email;
using GestionFacturas.Infrastructure.MongoDb;
using GestionFacturas.Infrastructure.Pdf;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License =
    LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirAngular", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));

builder.Services.AddScoped<IFacturaRepository,
    MongoFacturaRepository>();

builder.Services.AddScoped<IClienteRepository,
    MongoClienteRepository>();

builder.Services.AddScoped<IEmailService,
    EmailService>();

builder.Services.AddScoped<FacturaService>();

builder.Services.AddScoped<ClienteService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFacturaPdfService, FacturaPdfService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();