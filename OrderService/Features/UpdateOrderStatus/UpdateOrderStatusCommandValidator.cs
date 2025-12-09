using FluentValidation;
using OrderService.Domain.Enums;

namespace OrderService.Features.UpdateOrderStatus;

public sealed class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(c => c.NewStatus)
            .IsEnumName(typeof(Status), caseSensitive: false).WithMessage("Invalid status provided");
    }
}
