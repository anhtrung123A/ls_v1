using app.API.Middlewares;
using app.Application.Mappings;
using app.Application.DTOs.Responses;
using app.Application.Errors;
using app.Application.UseCases;
using app.Application.Validators;
using Asp.Versioning;
using app.Domain.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using app.Infrastructure.Configurations;
using app.Domain.Constants;
using app.Infrastructure.ExternalServices;
using app.Infrastructure.Persistence;
using app.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Input JWT token. Example: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend5173", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

        var response = ApiResponse<Dictionary<string, string[]>>.Fail("Validation failed.");
        response.Data = errors;
        return new BadRequestObjectResult(response);
    };
});
builder.Services
    .AddOptions<AuthServiceOptions>()
    .Bind(builder.Configuration.GetSection(AuthServiceOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services
    .AddOptions<SmtpOptions>()
    .Bind(builder.Configuration.GetSection(SmtpOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services
    .AddOptions<StorageOptions>()
    .Bind(builder.Configuration.GetSection(StorageOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var jwtSecret = builder.Configuration[$"{JwtOptions.SectionName}:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var principal = context.Principal;
                var email = principal?.FindFirstValue(JwtClaimNames.Email) ?? principal?.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrWhiteSpace(email) || principal is null)
                {
                    return;
                }

                var hasRoleIdClaim = principal.HasClaim(x => x.Type == JwtClaimNames.RoleId);
                if (hasRoleIdClaim)
                {
                    return;
                }

                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                var roleId = await userRepository.GetRoleIdByEmailAsync(email, context.HttpContext.RequestAborted);
                if (!roleId.HasValue)
                {
                    return;
                }

                if (principal.Identity is ClaimsIdentity identity)
                {
                    identity.AddClaim(new Claim(JwtClaimNames.RoleId, roleId.Value.ToString()));
                }
            },
            OnAuthenticationFailed = context =>
            {
                context.HttpContext.Items["AuthErrorMessage"] = context.Exception is SecurityTokenExpiredException
                    ? AppErrors.Auth.TokenExpired
                    : AppErrors.Auth.TokenInvalid;

                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var message = context.HttpContext.Items.TryGetValue("AuthErrorMessage", out var authError)
                    && authError is string authErrorMessage
                    ? authErrorMessage
                    : AppErrors.Auth.Unauthorized;

                var response = ApiResponse<object>.Fail(message);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BranchCrudRoleOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(JwtClaimNames.RoleId, "2");
    });
});
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
builder.Services.AddAutoMapper(typeof(BranchMappingProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddHttpClient<IExternalApiClient, ExternalApiClient>();
builder.Services.AddScoped<IAuthUserService, AuthUserService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IImageProcessor, ImageSharpProcessor>();
builder.Services.AddScoped<IFileStorageService, MinioFileStorageService>();
builder.Services.AddScoped<IFileUrlResolver, StorageFileUrlResolver>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<CreateUserUseCase>();
builder.Services.AddScoped<GetUserProfileUseCase>();
builder.Services.AddScoped<UpsertUserAvatarUseCase>();
builder.Services.AddScoped<DeleteUserAvatarUseCase>();
builder.Services.AddScoped<CreateBranchUseCase>();
builder.Services.AddScoped<GetBranchesUseCase>();
builder.Services.AddScoped<GetBranchByIdUseCase>();
builder.Services.AddScoped<UpdateBranchUseCase>();
builder.Services.AddScoped<DeleteBranchUseCase>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");

    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend5173");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
