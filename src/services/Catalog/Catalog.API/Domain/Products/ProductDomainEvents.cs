using Kernel.Interfaces;

namespace Catalog.Domain;

public record ProducPublished(Product Product) : IDomainEvent;


// Status events
public record ProductDeactivated(Guid ProductId) : IDomainEvent;
public record ProductReactivated(Guid ProductId) : IDomainEvent;


// Text information events
public record ProductNameEdited(Guid ProductId, string Name) : IDomainEvent;
public record ProductShortDescriptionEdited(Guid ProductId, string ShortDescription) : IDomainEvent;
public record ProductDescriptionEdited(Guid ProductId, string Description) : IDomainEvent;


// Pricing events
public record ProductCostChanged(Guid ProductId, decimal Cost) : IDomainEvent;
public record ProductPriceChanged(Guid ProductId, decimal Price) : IDomainEvent;


// Image Events
public record ProductImageAdded(Guid ProductId, string Image) : IDomainEvent;
public record ProductMainImageChanged(Guid ProductId, string MainImage) : IDomainEvent;
public record ProductSecondaryImageChanged(Guid ProductId, string SecondaryImage) : IDomainEvent;
public record ProductImageRemoved(Guid ProductId, string Image) : IDomainEvent;


// Attribute events
public record ProductAttributeAdded(
    Guid ProductId,
    Guid AttributeId,
    string AttributeName,
    int DisplayOrder,
    bool GroupVariants,
    Dictionary<Guid, AttributeValue> VariantAttributeValues // VariantId - new AttributeValue
    ) : IDomainEvent;
public record ProductAttributeRemoved(Guid ProductId, Guid AttributeId) : IDomainEvent;


// Variant events
public record ProductVariantAdded(Guid ProductId, Variant Variant) : IDomainEvent;
public record ProductVariantNameEdited(Guid ProductId, Guid VariantId, string Name) : IDomainEvent;
public record ProductVariantSkuEdited(Guid ProductId, Guid VariantId, string Sku) : IDomainEvent;
public record ProductVariantCostChanged(Guid ProductId, Guid VariantId, decimal Cost) : IDomainEvent;
public record ProductVariantPriceChanged(Guid ProductId, Guid VariantId, decimal Price) : IDomainEvent;
public record ProductVariantDimensionsChanged(Guid ProductId, Guid VariantId, ProductDimensions Dimensions) : IDomainEvent;
public record ProductVariantAttributeValueChanged(Guid ProductId, Guid VariantId, Guid AttributeId, AttributeValue Value) : IDomainEvent;
public record ProductVariantImageAdded(Guid ProductId, Guid VariantId, string Image) : IDomainEvent;
public record ProductVariantMainImageChanged(Guid ProductId, Guid VariantId, string Image) : IDomainEvent;
public record ProductVariantImageRemoved(Guid ProductId, Guid VariantId, string Image) : IDomainEvent;
public record ProductVariantDeactivated(Guid ProductId, Guid VariantId) : IDomainEvent;
public record ProductVariantReactivated(Guid ProductId, Guid VariantId) : IDomainEvent;
public record ProductVariantRemoved(Guid ProductId, Guid VariantId) : IDomainEvent;


// Other events
public record ProductCategoryChanged(Guid ProductId, Guid CategoryId, string CategoryName) : IDomainEvent;
public record ProductDimensionsChanged(Guid ProductId, ProductDimensions Dimensions) : IDomainEvent;