using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using SmartCharge;
using SmartCharge.DataLayer;
using SmartCharge.Handlers.ChargeStation;
using SmartCharge.Handlers.Connector;
using SmartCharge.Handlers.Group;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer("Server=localhost;Database=smartchargedb;User Id=sa;Password=Asd123456;TrustServerCertificate=True");
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ApplicationDbContext>());

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateGroupHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateChargeStationHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateConnectorHandler).Assembly));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IGroupRepository, GroupRepository>();
builder.Services.AddTransient<IChargeStationRepository, ChargeStationRepository>();
builder.Services.AddTransient<IConnectorRepository, ConnectorRepository>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

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