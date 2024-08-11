using FluentValidation;
using UserService.Requests;

namespace UserService.Validators;

public class UpdateVisibilityRequestValidator : AbstractValidator<UpdateVisibilityRequest>
{
    public UpdateVisibilityRequestValidator()
    {
        RuleFor(r => r.Visibility)
            .IsInEnum()
            .WithMessage("Visibility must be either 'Public' or 'Private'.");
    }
}