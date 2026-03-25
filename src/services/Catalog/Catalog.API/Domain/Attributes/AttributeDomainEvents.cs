using Kernel.Interfaces;

namespace Catalog.Domain;

public record AttributeCreated(Attribute Attribute) : IDomainEvent;
public record AttributeNameEdited(Guid AttributeId, string Name) : IDomainEvent;
public record AttributeDeleted(Guid AttributeId);
public record AttributeValueAdded(Guid AttributeId, AttributeValue Value) : IDomainEvent;
public record AttributeValueRemoved(Guid AttributeId, string ValueText) : IDomainEvent;
