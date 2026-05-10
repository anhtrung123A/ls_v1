using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using app.Configurations;
using app.Data.EF;
using app.Data.Redis;
using app.Common.Filters;
using app.Middlewares;
using app.Repositories.Staff;
using app.Repositories.Users;
using app.Repositories.Leads;
using app.Repositories.Students;
using app.Repositories.Interactions;
using app.Repositories.Tasks;
using app.Repositories.Courses;
using app.Repositories.CourseCategories;
using app.Repositories.Rooms;
using app.Repositories.Classes;
using app.Repositories.ClassSchedules;
using app.Repositories.Enrollments;
using app.Repositories.Invoices;
using app.Repositories.Payments;
using app.Repositories.ClassAttendances;
using app.Repositories.SalaryConfigs;
using app.Repositories.Payrolls;
using app.Services.Auth;
using app.Services.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Input JWT access token in this format: Bearer {your token}"
        };

        return Task.CompletedTask;
    });
});
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing connection string 'ConnectionStrings:Default'.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var redisConnectionString = builder.Configuration["Redis:Connection"]
    ?? throw new InvalidOperationException("Missing redis connection string 'Redis:Connection'.");

RedisConnection.Init(redisConnectionString);
builder.Services.AddScoped<RedisService>();
builder.Services.AddScoped<ApiResponseFilter>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("Missing JWT configuration section.");

if (string.IsNullOrWhiteSpace(jwtOptions.Secret) || jwtOptions.Secret.Length < 32)
{
    throw new InvalidOperationException("Jwt:Secret must be set and at least 32 characters long.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILeadRepository, LeadRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IInteractionRepository, InteractionRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IClassScheduleRepository, ClassScheduleRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IClassAttendanceRepository, ClassAttendanceRepository>();
builder.Services.AddScoped<ISalaryConfigRepository, SalaryConfigRepository>();
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();

var app = builder.Build();
app.Logger.LogInformation("MySQL connection: {ConnectionString}", MaskSensitiveConnectionString(connectionString));
app.Logger.LogInformation("Redis connection: {RedisConnection}", MaskSensitiveConnectionString(redisConnectionString));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapScalarApiReference();

var httpsPort = app.Configuration["ASPNETCORE_HTTPS_PORT"] ?? app.Configuration["HTTPS_PORT"];
if (!string.IsNullOrWhiteSpace(httpsPort))
{
    app.UseHttpsRedirection();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var api = app.MapGroup(string.Empty)
    .AddEndpointFilter<ApiResponseFilter>();

api.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

static string MaskSensitiveConnectionString(string connectionString)
{
    var segments = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);

    for (var i = 0; i < segments.Length; i++)
    {
        var pair = segments[i].Split('=', 2, StringSplitOptions.TrimEntries);
        if (pair.Length != 2)
        {
            continue;
        }

        if (pair[0].Equals("Password", StringComparison.OrdinalIgnoreCase) ||
            pair[0].Equals("Pwd", StringComparison.OrdinalIgnoreCase))
        {
            segments[i] = $"{pair[0]}=***";
        }
    }

    return string.Join(';', segments);
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
