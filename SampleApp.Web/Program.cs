using Domain.Aggregates;
using Domain.Read;
using Domain.Write.Events;
using Domain.Write.Events.Handlers;
using Framework.Core;
using Framework.Impl;
using SampleApp.Web;

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

builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();

builder.Services.RegisterHandlers(typeof(ProductAggregate).Assembly);

builder.Services.AddSingleton<AggregateRepository<ProductAggregate>>();

builder.Services.AddSingleton<IReadRepository<ProductReadModel>, ProductReadRepository>();

builder.Services.AddSingleton<IEventListener, EventListener<ProductReadModel>>(provider =>
{
    // Get the required services from the service provider
    var readRepository = provider.GetRequiredService<IReadRepository<ProductReadModel>>();
    var eventStore = provider.GetRequiredService<IEventStore>();

    // Create the EventListener instance
    var eventListener = new EventListener<ProductReadModel>(eventStore, readRepository);

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
