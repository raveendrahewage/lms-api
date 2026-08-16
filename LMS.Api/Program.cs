using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using LMS.Api.Helpers;
using LMS.Api.Helpers.Interfaces;
using LMS.Api.Middleware;
using LMS.Api.OpenApi.Transformers;
using LMS.Api.SignalR;
using LMS.Data;
using LMS.Data.Models;
using LMS.Service.Services;
using LMS.Services;
using LMS.Services.Azure.Interfaces;
using LMS.Services.Interfaces;
using LMS.Services.Mappings;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.ConfigureKeyVault(builder.Environment.IsDevelopment());

var JWTSetting = builder.Configuration.GetSection("JWTSetting");

var config = TypeAdapterConfig.GlobalSettings;

// Scan the assembly where MapsterConfig lives
config.Scan(typeof(MapsterRegistry).Assembly);
config.Default.PreserveReference(true);
config.Compile();

builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddSingleton(_ => new BlobServiceClient(builder.Configuration.GetConnectionString("BlobStorageConnection")));
builder.Services.AddSingleton(_ => new ServiceBusClient(builder.Configuration.GetConnectionString("ServiceBusConnection")));

builder.Services.AddScoped<IAzureStorageService, AzureStorageService>();

// signalR
builder.Services
    .AddSignalR()
    .AddAzureSignalR(builder.Configuration.GetConnectionString("SignalRConnection"));
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddScoped<INotificationPublisher, SignalRNotificationPublisher>();

// Add services to the container.
builder.Services.AddDbContext<DbContext, ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("LMSDbConnection"));
    }
);

builder.Services.AddIdentity<SystemUser, SystemRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 0;

        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.";
        options.User.RequireUniqueEmail = true;
    }
)
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }
).AddJwtBearer(opt =>
    {
        opt.SaveToken = true;
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            //ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = JWTSetting["ValidAudience"],
            ValidIssuer = JWTSetting["ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSetting.GetSection("SecurityKey").Value!))
        };
        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/leave-notifications"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://thankful-moss-0159cd01e.7.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddSingleton<IApiResponseHelper, ApiResponseHelper>();

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

builder.Services.AddScoped<INotificationPublisher, SignalRNotificationPublisher>();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILeaveTypeService, LeaveTypeService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<INotificationService, NotificationService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("SignalRCorsPolicy");

app.UseMiddleware<ApiExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "API is running...");

app.MapHub<LeaveNotificationHub>("/hubs/leave-notifications");

app.Run();
