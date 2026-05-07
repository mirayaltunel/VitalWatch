using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Hubs;
using VitalWatch.Api.Services.Abstract;
using VitalWatch.Api.Services.Concrete;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetSection("ConnectionString").Value;
builder.Services.AddDbContext<VitalWatchDbContext>(opt => opt.UseNpgsql(connectionString));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<ISensorMeasurementService, SensorMeasurementService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IDiseaseService, DiseaseService>();
builder.Services.AddScoped<IHealthEventService, HealthEventService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPatientDiseaseService, PatientDiseaseService>();
builder.Services.AddScoped<ISeizureEventService, SeizureEventService>();
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<IUserPatientService, UserPatientService>();
builder.Services.AddSingleton<SimulationService>();
builder.Services.AddSignalR();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<VitalHub>("/hubs/vital");

app.Run("http://0.0.0.0:5000");
