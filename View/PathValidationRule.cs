using System;
using System.Globalization;
using System.Windows.Controls;
using FileArchiver.Base;
using FileArchiver.Services;

namespace FileArchiver.View {
    public class PathValidationRule : ValidationRule {
        readonly DefaultInputDataService service;

        public PathValidationRule() {
            this.service = new DefaultInputDataService(new DefaultPlatformService());
        }
        public override ValidationResult Validate(object value, CultureInfo culture) {
            string inputPath = (string)value;

            if(!string.IsNullOrEmpty(inputPath)) {
                InputType inputType = service.GetInputType(inputPath);
                if(inputType == InputType.Unknown) return new ValidationResult(false, $"Invalid Path");
            }
            return ValidationResult.ValidResult;
        }
    }
}