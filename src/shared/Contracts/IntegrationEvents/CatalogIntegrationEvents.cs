using Kernel.Interfaces;

namespace Catalog.IntegrationEvents;

public record ProductPublished(
    Guid ProductId,
    string Name,
    string MainImage,
    List<VariantInfo> Variants) : IIntegrationEvent; // ==> Init Inventory Items

public record VariantInfo(Guid VariantId, string Name, string MainImage) : IIntegrationEvent;

public record ProductDeactivated(Guid ProductId) : IIntegrationEvent; // ==> Lock Inventory Items
public record ProductReactivated(Guid ProductId) : IIntegrationEvent; // ==> Unlock Inventory Items
public record ProductVariantDeactivated(Guid ProductId, Guid VariantId) : IIntegrationEvent; // ==> Lock Inventory Items
public record ProductVariantReactivated(Guid ProductId, Guid VariantId) : IIntegrationEvent; // ==> Unlock Inventory Items