
using DataStorageCore.Repositories;
using DataStorageDataAccess;
using DataStorageDataAccess.DataBaseContext;
using DataStorageWebApi.Services;
using DataStorageWebApi.TaskManager;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<EnergyContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionStrings__SQLConnectionString") ??
                      builder.Configuration.GetConnectionString("SQLConnectionString") ??
                      throw new Exception("No connection string to sql database"));

    options.UseLowerCaseNamingConvention();
});
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddSingleton(typeof(ITaskManager), typeof(TaskManager));
builder.Services.AddScoped(typeof(DbContext), typeof(EnergyContext));
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped(typeof(IDataFromDeviceService), typeof(DataFromDeviceService));

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
