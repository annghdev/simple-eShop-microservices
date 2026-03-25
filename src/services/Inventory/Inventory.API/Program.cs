using JasperFx;
using Marten.Events.Projections;
using Scalar.AspNetCore;
using Wolverine.Configuration;
using Wolverine.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWolverine(opts =>
{
    opts.Policies.AutoApplyTransactions();

    opts.Policies
        .OnException<ConcurrencyException>()
        .RetryWithCooldown(50.Milliseconds(), 250.Milliseconds(), 1.Seconds())
        .Then.MoveToErrorQueue();

    // Outbox
    opts.Policies.UseDurableLocalQueues();

    opts.MessagePartitioning
    .ByMessage<IReservationEvent>(x => x.OrderId.ToString())
    .PublishToPartitionedLocalMessaging("reservation", 4, topology =>
    {
        topology.MessagesImplementing<IReservationEvent>();

        topology.MaxDegreeOfParallelism = PartitionSlots.Five;

        topology.ConfigureQueues(queue =>
        {
            queue.TelemetryEnabled(true);
        });
    });

});

builder.AddServiceDefaults();

builder.Services.AddWolverineHttp();

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("inventorydb")!);
    opts.Projections.Snapshot<InventoryItem>(SnapshotLifecycle.Inline);
})
.IntegrateWithWolverine();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (!app.Environment.IsDevelopment())
{
    //app.UseHttpsRedirection();
}
app.MapWolverineEndpoints();

app.MapGet("/", () => Results.Redirect("scalar/v1"));

app.Run();
