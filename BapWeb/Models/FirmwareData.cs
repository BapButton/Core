using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BapWeb.Models
{

    public class FileValidationAttribute : ValidationAttribute
    {
        public FileValidationAttribute(string[] allowedExtensions)
        {
            AllowedExtensions = allowedExtensions;
        }

        private string[] AllowedExtensions { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = (IBrowserFile)value;

            var extension = System.IO.Path.GetExtension(file.Name);

            if (!AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return new ValidationResult($"File must have one of the following extensions: {string.Join(", ", AllowedExtensions)}.", new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }


    public class FileUpload
    {
        [Required]
        [StringLength(255, MinimumLength = 2)]
        public string Description { get; set; } = $"Firmware uploaded on {DateTime.Today.ToShortDateString()}";
        [Required]
        public string Version { get; set; } = "";
        public bool ShowOverwriteButton { get; set; }
        public bool OverWriteCurrentFirmware { get; set; }

        [Required]
        [FileValidation(new[] { ".bin" })]
        public IBrowserFile Firmware { get; set; }
    }

    public class FileUploadValidator : AbstractValidator<FileUpload>
    {
        public FileUploadValidator()
        {
            RuleFor(p => p.Version)
            .NotEmpty().WithMessage("You must enter a version")
            .MaximumLength(10).WithMessage("Firmware version can be no longer then 10 characters")
            .Must(t => t.Split('.').Count() == 3).WithMessage("A version must contain two periods.");
        }
    }
}

