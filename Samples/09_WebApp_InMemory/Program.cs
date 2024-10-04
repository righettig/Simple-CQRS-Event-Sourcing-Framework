using Domain.Aggregates;
using Domain.Read;
using Framework.Core;
using Framework.Impl;
using Framework.Web;

/**
 * This sample shows how to use the in-memory implementation in a web application.
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

builder.Services.AddEventStore(); // use an in-memory implementation of IEventStore

builder.Services.RegisterHandlers(typeof(ProductAggregate).Assembly);

builder.Services.AddSingleton<AggregateRepository<ProductAggregate>>();
builder.Services.AddSingleton<IReadRepository<ProductReadModel>, ProductReadRepository>();

// This will automatically register all events to corresponding event handlers based on convention
// <<EVENT_NAME>>Event => <<EVENT_NAME>>EventHandler
builder.Services.AddEventListener<ProductReadModel>(typeof(ProductReadModel).Assembly);

// Alternatively, events can be manually bound to event handlers by doing:
//builder.Services.AddSingleton<IEventListener, EventListener<ProductReadModel>>(provider =>
//{
//    // Get the required services from the service provider
//    var readRepository = provider.GetRequiredService<IReadRepository<ProductReadModel>>();

//    // Create the EventListener instance
//    var eventListener = new EventListener<ProductReadModel>(readRepository);

//    // Bind the event handlers
//    eventListener.Bind<ProductCreatedEvent, ProductCreatedEventHandler>();
//    eventListener.Bind<ProductPriceUpdatedEvent, ProductPriceUpdatedEventHandler>();
//    eventListener.Bind<ProductDeletedEvent, ProductDeletedEventHandler>();

//    return eventListener;
//});

builder.Services.AddHostedService<EventListenerBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
