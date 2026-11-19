using CodeInvention.CuriosityLab.Application;
using CodeInvention.CuriosityLab.Infrastructure;
using CodeInvention.CuriosityLab.Persistence;
using CodeInvention.CuriosityLab.Web.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddPresentation(builder.Configuration);
builder.Services.AddInfrastructure();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!builder.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UsePresentation();
app.UseCors("Frontend");
app.MapControllers();

app.Run();
