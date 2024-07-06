using HahnTransportAutomate.DAL;
using HahnTransportAutomate.DAL.IRepositories;
using HahnTransportAutomate.DAL.Repositories;
using HahnTransportAutomate.Services;
using HahnTransportAutomate.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<HahnTransDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("HahnCon")),ServiceLifetime.Scoped);
//builder.Services.AddScoped<ITokenRepository, TokenRepository>();
//builder.Services.AddScoped<ItransporterRepository, TransporterRepository>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IOrderManager, OrderManager>();
//builder.Services.AddScoped<IOrderService, OrderService>();
//builder.Services.AddScoped<ICargoTransporterService, CargoTransporterService>();
//builder.Services.AddScoped<IGridService, GridService>();
//builder.Services.AddScoped<ISimService, SimService>();
//builder.Services.AddScoped<IStrategyService, StrategyService>();

//builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"));
//builder.Services.AddSingleton<SimServer>();

//builder.Services.AddHttpClient("HahnSim", client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration["LoginEndPoint"]);
//});
//builder.Services.AddHostedService<SimServer>();
//builder.Services.AddCors();



builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"));
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IOrderManager, OrderManager>();
builder.Services.AddSingleton<ISimService, SimService>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddSingleton<ICargoTransporterService, CargoTransporterService>();
builder.Services.AddSingleton<IGridService, GridService>();
builder.Services.AddSingleton<IStrategyService, StrategyService>();


builder.Services.AddDbContext<HahnTransDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("HahnCon")), ServiceLifetime.Singleton);


builder.Services.AddSingleton<ITokenRepository, TokenRepository>();
builder.Services.AddSingleton<ItransporterRepository, TransporterRepository>();
builder.Services.AddSingleton<SimServer>();

builder.Services.AddHttpClient("HahnSim", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["LoginEndPoint"]);
});
builder.Services.AddHostedService<SimServer>();
builder.Services.AddCors();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(builder =>
{
    builder
      .AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
});


app.MapControllers();

app.Run();
