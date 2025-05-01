using Microsoft.EntityFrameworkCore;
using ClienteApi.Core.Domain.Interfaces;
using ClienteApi.Application.Interfaces;
using ClienteApi.Application.UseCases;
using ClienteApi.Infrastructure.Persistence.Repositories;
using ClienteApi.Infrastructure.Persistence.Contexts;
using ClienteApi.Infrastructure.ExternalServices.ViaCepService;
using ClienteApi.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Garantir que o appsettings.json seja carregado corretamente
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Configurar a conexão com o banco
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("A string de conexão com o banco não foi encontrada.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention();  // Aplica a convenção de snake_case
});

// Registrar os serviços necessários
builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IViaCepService, ViaCepService>();
builder.Services.AddScoped<IClienteTRA, ClienteTRA>();
builder.Services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();

// Registrar o HttpClient se necessário para outros serviços
builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar pipeline de requisição
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();