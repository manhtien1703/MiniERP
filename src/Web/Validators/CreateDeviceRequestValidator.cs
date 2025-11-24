using FluentValidation;
using Web.Models;

namespace Web.Validators;

public class CreateDeviceRequestValidator : AbstractValidator<CreateDeviceRequest>
{
    public CreateDeviceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.DeviceType)
            .IsInEnum().WithMessage("Invalid DeviceType value.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("WarehouseId is required.");
    }
}

