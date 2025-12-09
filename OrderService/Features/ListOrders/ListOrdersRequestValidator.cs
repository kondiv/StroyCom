using FluentValidation;

namespace OrderService.Features.ListOrders;

public sealed class ListOrdersRequestValidator : AbstractValidator<ListOrdersRequest>
{
    public ListOrdersRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than 0");

        RuleFor(r => r.MaxPageSize)
            .GreaterThan(0).WithMessage("At least one element required")
            .LessThan(50).WithMessage("Max 50 elements on page");
    }
}
