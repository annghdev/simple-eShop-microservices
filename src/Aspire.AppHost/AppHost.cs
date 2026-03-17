var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.APIGateway>("apigateway");

builder.AddProject<Projects.Catalog_API>("catalog-api");

builder.AddProject<Projects.Inventory_API>("inventory-api");

builder.AddProject<Projects.Order_API>("order-api");

builder.Build().Run();
