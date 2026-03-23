namespace Inventory.Domain;

public record WarehouseCreated(Guid Id, string Name, decimal Latitude, decimal Longitude);
public record WarehouseDeactivated(Guid Id);
public record WarehouseReactivated(Guid Id);
public record WarehouseNameEdited(Guid Id, string Name);
public record WarehouseLocationChanged(Guid Id, decimal Latitude, decimal Longitude);