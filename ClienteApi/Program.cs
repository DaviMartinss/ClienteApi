using ClienteApi.Data.Contexts;
using ClienteApi.Data.Repositories;
using ClienteApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar a conexão com o banco
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// Adicionar serviços existentes
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddHttpClient<ViaCepService>();
builder.Services.AddSingleton<RabbitMQProducer>();

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