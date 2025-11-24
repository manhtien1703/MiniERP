using FluentValidation;
using Web.Models;

namespace Web.Validators;

public class UpdateDeviceRequestValidator : AbstractValidator<UpdateDeviceRequest>
{
    public UpdateDeviceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.DeviceType)
            .IsInEnum().WithMessage("Invalid DeviceType value.");
    }
}

