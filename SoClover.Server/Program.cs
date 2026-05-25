using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SoClover.Server.Context;
using SoClover.Server.Filters;
using SoClover.Server.Helpers;
using SoClover.Server.Hubs;
using SoClover.Server.Services;
using System.Text.Json.Serialization;
using log4net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("SoCloverPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:54861")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

//Logging
builder.Services.AddLogging(
    x => x.AddLog4Net()
);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR(options => options.AddFilter<HubErrorFilter>())
    .AddJsonProtocol(options => {
        options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Register application services
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddTransient<IRoomService, RoomService>();

builder.Services.AddHostedService<DatabaseCleanupService>();

//Helpers
builder.Services.AddTransient<ICardManager, CardManager>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SoCloverDBContext>(options =>
    options.UseSqlServer(connectionString)
    .EnableSensitiveDataLogging());
var app = builder.Build();

app.UseCors("SoCloverPolicy");

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<SoCloverHub>("/socloverhub");

app.Run();
