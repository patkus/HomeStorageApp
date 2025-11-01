using FluentValidation;
using HomeStorageApp.Drugs.Core.Application.DTOs;

namespace HomeStorageApp.Drugs.Api.Validators;

/// <summary>
/// Validator dla UpdateDrugRequest
/// </summary>
public sealed class UpdateDrugRequestValidator : AbstractValidator<UpdateDrugRequest>
{
    /// <summary>
    /// Konstruktor definiujący reguły walidacji
    /// </summary>
    public UpdateDrugRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Nazwa nie może być dłuższa niż 200 znaków")
            .Matches("^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ0-9 -]+$")
            .WithMessage("Nazwa zawiera niedozwolone znaki")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
            
        RuleForEach(x => x.Barcodes)
            .MaximumLength(100).WithMessage("Kod kreskowy nie może być dłuższy niż 100 znaków")
            .Matches("^[0-9A-Z-]+$").WithMessage("Kod kreskowy zawiera niedozwolone znaki")
            .When(x => x.Barcodes is not null);
            
        RuleForEach(x => x.DerivedUnits)
            .SetValidator(new UpdateDerivedUnitDtoValidator())
            .When(x => x.DerivedUnits is not null);
    }
}

/// <summary>
/// Validator dla UpdateDerivedUnitDto
/// </summary>
public sealed class UpdateDerivedUnitDtoValidator : AbstractValidator<UpdateDerivedUnitDto>
{
    /// <summary>
    /// Konstruktor definiujący reguły walidacji
    /// </summary>
    public UpdateDerivedUnitDtoValidator()
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
