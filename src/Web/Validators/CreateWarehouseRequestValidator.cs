using FluentValidation;
using Web.Models;

namespace Web.Validators;

public class CreateWarehouseRequestValidator : AbstractValidator<CreateWarehouseRequest>
{
    public CreateWarehouseRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(500).WithMessage("Location must not exceed 500 characters.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0.")
            .LessThanOrEqualTo(1000000).WithMessage("Capacity must not exceed 1,000,000.");

        RuleFor(x => x.ProvinceId)
            .GreaterThan(0).WithMessage("ProvinceId must be greater than 0.");
    }
}

