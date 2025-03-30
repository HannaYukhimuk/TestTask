using FluentValidation;
using Library.Domain.Models;

public class BookCreateDtoValidator : AbstractValidator<BookCreateDto>
{
    public BookCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN is required")
            .Matches(@"^\d{3}-\d{1,2}-\d{1,5}-\d{1,6}-\d$").WithMessage("Invalid ISBN format. Example: 978-3-16-144110-0");

        RuleFor(x => x.Genre)
            .NotEmpty().WithMessage("Genre is required");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Author's first name is required");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Author's last name is required");
    }
}
