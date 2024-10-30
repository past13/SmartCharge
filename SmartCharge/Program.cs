using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using SmartCharge;
using SmartCharge.DataLayer;
using SmartCharge.Handlers;
using SmartCharge.Handlers.ChargeStation;
using SmartCharge.Handlers.Connector;
using SmartCharge.Handlers.Group;
using SmartCharge.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddMediatR(typeof(CreateGroupHandler).Assembly); 
builder.Services.AddMediatR(typeof(UpdateGroupHandler).Assembly); 
builder.Services.AddMediatR(typeof(DeleteGroupHandler).Assembly); 

builder.Services.AddMediatR(typeof(CreateChargeStationHandler).Assembly); 
builder.Services.AddMediatR(typeof(UpdateChargeStationHandler).Assembly); 
builder.Services.AddMediatR(typeof(DeleteChargeStationHandler).Assembly); 

builder.Services.AddMediatR(typeof(CreateConnectorHandler).Assembly); 
builder.Services.AddMediatR(typeof(UpdateConnectorHandler).Assembly); 
builder.Services.AddMediatR(typeof(DeleteConnectorHandler).Assembly); 

builder.Services.AddTransient<IGroupRepository, GroupRepository>();
builder.Services.AddTransient<IChargeStationRepository, ChargeStationRepository>();
builder.Services.AddTransient<IConnectorRepository, ConnectorRepository>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=smartchargedb;Username=smartchargeuser;Password=smartchargepassword;");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = ""; 
    });

    app.MapScalarApiReference(); // Map Scalar API reference
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();