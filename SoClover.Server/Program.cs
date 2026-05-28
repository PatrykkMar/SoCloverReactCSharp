using log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoClover.Server.Context;
using SoClover.Server.Filters;
using SoClover.Server.Helpers;
using SoClover.Server.Hubs;
using SoClover.Server.Services;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

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
builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SoCloverDBContext>(options =>
    options.UseSqlServer(connectionString)
    .EnableSensitiveDataLogging());


var jwtSecret = builder.Configuration["JwtSettings:Secret"]
    ?? throw new Exception("JWT Secret is missing in appsettings.json");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,  
        ValidateLifetime = true, 
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/socloverhub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SoCloverHub>("/socloverhub");

app.Run();


public class NameIdentifierUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}