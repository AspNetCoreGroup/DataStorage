
using DataStorageCore.Repositories;
using DataStorageDataAccess;
using DataStorageDataAccess.DataBaseContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<EnergyContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("SQLConnectionString"));
    options.UseLowerCaseNamingConvention();
});

builder.Services.AddScoped(typeof(DbContext), typeof(EnergyContext));
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
