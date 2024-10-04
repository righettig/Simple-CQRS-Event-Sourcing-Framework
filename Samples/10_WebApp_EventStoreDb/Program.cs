using Domain.Aggregates;
using Domain.Read;
using Framework.Core;
using Framework.Impl;
using Framework.Web;

/**
 * This sample shows both write/read stack as well as projections being done in a single project.
 * The sample shows also how to process a subset of the events based on the event prefix.
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
builder.Services.AddSingleton<IReadRepository<ProductReadModel>, ProductReadRepository>();
builder.Services.AddEventListener<ProductReadModel>(typeof(ProductReadModel).Assembly);

// The background service will process all events
builder.Services.AddHostedService<EventListenerBackgroundService>();

// The background service will only process a subset of all the events
// events that do NOT start with "my_domain_products" will be ignored.
// Events are saved with the namespace prefixed so the match is based on the class namespace.
//builder.Services.AddEventListenerBackgroundService("my_domain_products");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
