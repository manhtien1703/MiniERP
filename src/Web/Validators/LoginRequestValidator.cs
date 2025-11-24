using FluentValidation;
using Web.Models;

namespace Web.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username là bắt buộc.")
            .MaximumLength(50).WithMessage("Username không được vượt quá 50 ký tự.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password là bắt buộc.")
            .MinimumLength(6).WithMessage("Password phải có ít nhất 6 ký tự.");
    }
}

