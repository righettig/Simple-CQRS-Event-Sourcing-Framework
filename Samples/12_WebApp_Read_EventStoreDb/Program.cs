using Domain.Events;
using Domain.Projections;
using Domain.Read;
using Domain.Read.Queries.Handlers;
using Framework.Core;
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

builder.Services.AddEventStore("esdb://localhost:2113?tls=false");

builder.Services.RegisterHandlers(typeof(ProductCreatedEventHandler).Assembly);

builder.Services.AddSingleton<IReadRepository<ProductReadModel>, ProductReadRepository>();

builder.Services.AddEventListener<ProductReadModel>(
    typeof(ProductCreatedEvent).Assembly, 
    typeof(ProductCreatedEventHandler).Assembly);

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
