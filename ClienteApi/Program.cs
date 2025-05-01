using ClienteApi.Data.Contexts;
using ClienteApi.Data.Contexts.Interfaces;
using ClienteApi.Data.Repositories;
using ClienteApi.Data.Repositories.Interfaces;
using ClienteApi.Services;
using ClienteApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Configurar a conexão com o banco
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"))
           .UseSnakeCaseNamingConvention();  // Aplica a convenção de snake_case
});

// Registrar os serviços necessários
builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IViaCepService, ViaCepService>();
// Registrar RabbitMQProducer como Singleton
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
