using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MusicTrackService.Data;
using MusicTrackService.Operations;
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

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JobNet Post API", Version = "v1" });
    c.OperationFilter<FileUploadOperation>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enable static files middleware

app.UseRouting();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MusicTrack API V1");
});

app.MapControllers();


app.Run();