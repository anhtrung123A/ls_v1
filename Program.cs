using app.API.Middlewares;
using app.Application.DTOs;
using app.Application.DTOs.Responses;
using app.Application.UseCases;
using app.Application.Validators;
using app.Domain.Interfaces;
using FluentValidation;
using app.Infrastructure.ExternalServices;
using app.Infrastructure.Persistence;
using app.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
builder.Services.AddHttpClient<IExternalApiClient, ExternalApiClient>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<CreateUserUseCase>();
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

var apiV1 = app.MapGroup("/api/v1");

apiV1.MapPost("/users", async (
    CreateUserDto dto,
    IValidator<CreateUserDto> validator,
    CreateUserUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var validationResult = await validator.ValidateAsync(dto, cancellationToken);
    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToArray();
        return Results.BadRequest(ApiResponse<string[]>.Fail(string.Join("; ", errors)));
    }

    var user = await useCase.ExecuteAsync(dto, cancellationToken);
    return Results.Created($"/api/v1/users/{user.Id}", ApiResponse<UserDto>.Ok(user, "User created successfully."));
})
.WithName("CreateUser")
.WithOpenApi();

app.Run();
