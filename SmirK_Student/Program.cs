using Microsoft.Extensions.Options;
using SmirK_Student.API.Features.DriverOrder.Services;
using SmirK_Student.Domain.Drivers.Algorithms;
using SmirK_Student.Domain.Drivers.Map.Containers;
using SmirK_Student.Engine.Configuration;
using SmirK_Student.Engine.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Привязка настроек/конфигов
builder.Services.Configure<MapSettingsConfig>(builder.Configuration.GetSection("MapSettings"));
builder.Services.Configure<SettingsConfig>(builder.Configuration.GetSection("Settings"));

// Привязка адаптера
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MapSettingsConfig>>().Value;
    return new DriverLocationAdapter<ClassicGrid, ManhattanRadialSearch>(new ClassicGrid(settings.Width, settings.Height));
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Подключаем дополнительный слой для ограничения чистоты запросов
app.UseMiddleware<RateLimitingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
