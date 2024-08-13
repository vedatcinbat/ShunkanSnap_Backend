using Microsoft.EntityFrameworkCore;
using MusicTrackService.Data;
using MusicTrackService.Services.MusicService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<MusicTrackDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("MusicTrackDb"));
});

builder.Services.AddScoped<IMusicTrackService, MusicTrackService.Services.MusicService.MusicTrackService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();