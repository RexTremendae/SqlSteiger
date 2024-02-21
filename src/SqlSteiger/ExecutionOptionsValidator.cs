namespace SqlSteiger;

using FluentValidation;

public class ExecutionOptionsValidator : AbstractValidator<ExecutionOptions>
{
    public ExecutionOptionsValidator()
    {
        RuleFor(_ => _.ConnectionString)
            .NotEmpty()
            .WithMessage("Connection string must be specified.");
    }
}
