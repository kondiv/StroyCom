using OrderService.Domain.Models;

namespace OrderService.Contracts;

public sealed record CreateOrderRequest(Position Position, decimal TotalSum);
