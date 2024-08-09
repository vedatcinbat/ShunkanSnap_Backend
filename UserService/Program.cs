using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using UserService.Services;
using UserService.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();


builder.Services.AddDbContext<UserDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("mySqlConnection"));
});
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();
builder.Services.AddSingleton<UserEventPublisher>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();