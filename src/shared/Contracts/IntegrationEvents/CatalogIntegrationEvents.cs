namespace Catalog.IntegrationEvents;

public record ProductPublished(Guid ProductId); // ==> Init Inventory Items
public record ProductDeactivated(Guid ProductId); // ==> Lock Inventory Items
public record ProductReactivated(Guid ProductId); // ==> Unlock Inventory Items
public record ProductVariantDeactivated(Guid ProductId, Guid VariantId); // ==> Lock Inventory Items
public record ProductVariantReactivated(Guid ProductId, Guid VariantId); // ==> Unlock Inventory Items