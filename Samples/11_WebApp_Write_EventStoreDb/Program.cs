using Domain.Aggregates;
using Framework.Impl;
using Framework.Web;

/**
 * This sample shows how to host the write stack based on EventStoreDb in a web application.
 */

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CQRS - Event Sourcing Framework
builder.Services.AddMediatR(cfg =>
{
    // Register all handlers from the assembly where your command/query handlers are defined
    cfg.RegisterServicesFromAssemblyContaining<ProductAggregate>();
});

builder.Services.AddEventStore("esdb://localhost:2113?tls=false");

builder.Services.RegisterHandlers(typeof(ProductAggregate).Assembly);

builder.Services.AddSingleton<AggregateRepository<ProductAggregate>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
