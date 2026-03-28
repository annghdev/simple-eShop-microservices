using Kernel.Interfaces;

namespace Order.Domain;

public record OrderCreated(Order Order) : IDomainEvent; // Build projection