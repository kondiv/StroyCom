using FluentValidation;
using OrderService.Domain.Enums;

namespace OrderService.Features.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(c => c.Position.Quantity)
            .GreaterThanOrEqualTo(1).WithMessage("At least 1 item must be");

        RuleFor(c => c.TotalSum)
            .GreaterThanOrEqualTo(0m).WithMessage("Total sum can not be negative");
    }
}
