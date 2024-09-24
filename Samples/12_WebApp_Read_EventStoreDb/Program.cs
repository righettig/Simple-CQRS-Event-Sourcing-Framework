using Domain.Events;
using Domain.Projections;
using Domain.Read;
using Domain.Read.Queries.Handlers;
using EventStore.Client;
using Framework.Core;
using Framework.Impl;
using Framework.Impl.EventStore;
using Framework.Web;

/**
 * This sample shows how to host the read/projections stacks based on EventStoreDb in a web application.
 */

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CQRS - Event Sourcing Framework
builder.Services.AddMediatR(cfg =>
{
    // Register all handlers from the assembly where your command/query handlers are defined
    cfg.RegisterServicesFromAssemblyContaining<ProductQueryHandler>();
});

var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
var eventStoreClient = new EventStoreClient(settings);
var eventStore = new EventStoreDb(eventStoreClient);

builder.Services.RegisterHandlers(typeof(ProductCreatedEventHandler).Assembly);

builder.Services.AddSingleton<IEventStore>(eventStore);
builder.Services.AddSingleton<IReadRepository<ProductReadModel>, ProductReadRepository>();
builder.Services.AddSingleton<IEventListener, EventListener<ProductReadModel>>(provider =>
{
    // Get the required services from the service provider
    var readRepository = provider.GetRequiredService<IReadRepository<ProductReadModel>>();
    var eventStore = provider.GetRequiredService<IEventStore>();

    // Create the EventListener instance
    var eventListener = new EventListener<ProductReadModel>(readRepository);

    // Bind the event handlers
    eventListener.Bind<ProductCreatedEvent, ProductCreatedEventHandler>();
    eventListener.Bind<ProductPriceUpdatedEvent, ProductPriceUpdatedEventHandler>();
    eventListener.Bind<ProductDeletedEvent, ProductDeletedEventHandler>();
    
    return eventListener;
});

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
