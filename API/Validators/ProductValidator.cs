using Core.DTOs.Requests;
using FluentValidation;

namespace API.Validators
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .MaximumLength(100)
                .WithMessage("Product name must not exceed 100 characters.");

            RuleFor(x => x.ProductDescription)
                .NotEmpty()
                .WithMessage("Product description is required.")
                .MaximumLength(500)
                .WithMessage("Product description must not exceed 500 characters.");

            RuleFor(x => x.ProductPrice)
                .GreaterThan(0)
                .WithMessage("Product price must be greater than 0.")
                .WithMessage("Product price must have up to 18 digits with 2 decimal places.");

            RuleFor(x => x.ProductPictureUrl)
                .NotEmpty()
                .WithMessage("Product picture URL is required.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Product picture URL must be a valid URL.")
                .MaximumLength(500)
                .WithMessage("Product picture URL must not exceed 500 characters.");

            RuleFor(x => x.ProductType)
                .NotEmpty()
                .WithMessage("Product type is required.")
                .MaximumLength(50)
                .WithMessage("Product type must not exceed 50 characters.");

            RuleFor(x => x.ProductBrand)
                .NotEmpty()
                .WithMessage("Product brand is required.")
                .MaximumLength(50)
                .WithMessage("Product brand must not exceed 50 characters.");

            RuleFor(x => x.ProductQuantityInStock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Product quantity in stock cannot be negative.");
        }
    }
}