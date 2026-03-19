using Marten;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("catalogdb")!);
})
    .IntegrateWithWolverine();

builder.Services.AddWolverineHttp();

builder.Host.UseWolverine(opts =>
{
    opts.Policies.AutoApplyTransactions();
});

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
    app.UseHttpsRedirection();
}

app.MapWolverineEndpoints();

app.MapGet("/", () => Results.Redirect("scalar/v1"));

app.Run();
