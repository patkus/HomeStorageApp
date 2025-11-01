using FluentValidation;
using HomeStorageApp.Drugs.Core.Application.DTOs;

namespace HomeStorageApp.Drugs.Api.Validators;

/// <summary>
/// Validator dla CreateDrugRequest
/// </summary>
public sealed class CreateDrugRequestValidator : AbstractValidator<CreateDrugRequest>
{
    /// <summary>
    /// Konstruktor definiujący reguły walidacji
    /// </summary>
    public CreateDrugRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa leku jest wymagana")
            .MaximumLength(200).WithMessage("Nazwa nie może być dłuższa niż 200 znaków")
            .Matches("^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ0-9 -]+$")
            .WithMessage("Nazwa zawiera niedozwolone znaki");
            
        RuleFor(x => x.PrimaryUnitId)
            .NotEmpty().WithMessage("Jednostka główna jest wymagana");
            
        RuleForEach(x => x.Barcodes)
            .MaximumLength(100).WithMessage("Kod kreskowy nie może być dłuższy niż 100 znaków")
            .Matches("^[0-9A-Z-]+$").WithMessage("Kod kreskowy zawiera niedozwolone znaki")
            .When(x => x.Barcodes is not null);
            
        RuleForEach(x => x.DerivedUnits)
            .SetValidator(new CreateDerivedUnitDtoValidator())
            .When(x => x.DerivedUnits is not null);
    }
}

/// <summary>
/// Validator dla CreateDerivedUnitDto
/// </summary>
public sealed class CreateDerivedUnitDtoValidator : AbstractValidator<CreateDerivedUnitDto>
{
    /// <summary>
    /// Konstruktor definiujący reguły walidacji
    /// </summary>
    public CreateDerivedUnitDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa jednostki jest wymagana")
            .MaximumLength(100).WithMessage("Nazwa jednostki nie może być dłuższa niż 100 znaków");
            
        RuleFor(x => x.BaseUnitId)
            .NotEmpty().WithMessage("Jednostka bazowa jest wymagana");
            
        RuleFor(x => x.ConversionFactor)
            .GreaterThan(0).WithMessage("Przelicznik musi być większy od 0");
            
        RuleForEach(x => x.Barcodes)
            .MaximumLength(100).WithMessage("Kod kreskowy nie może być dłuższy niż 100 znaków")
            .Matches("^[0-9A-Z-]+$").WithMessage("Kod kreskowy zawiera niedozwolone znaki")
            .When(x => x.Barcodes is not null);
    }
}
