using Kernel.Interfaces;

namespace Catalog.Domain;

public record CategoryNameEdited(Guid Id, string Name) : IDomainEvent;
public record CategoryIconChanged(Guid Id, string Icon) : IDomainEvent;