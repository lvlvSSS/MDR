using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace MDR.Server.Samples.Models;

public class CreateUserDto
{
     public CreateUserNameDto? Name { get; set; }

    public int Age { get; set; }
}

public class CreateUserNameDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class CreateUserNameDtoValidator : AbstractValidator<CreateUserNameDto>
{
    public CreateUserNameDtoValidator()
    {
        RuleFor(x => x.FirstName).NotNull().NotEmpty();
        RuleFor(x => x.LastName).NotNull().NotEmpty();
    }
}

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Name).SetValidator(new CreateUserNameDtoValidator()!);
        RuleFor(x => x.Age).GreaterThan(0);
    }
}