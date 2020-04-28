using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace Library_Management_System.Validations
{
    class ValidationRuleForPayingFine : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            float RemainingAmount;
            if (float.TryParse(value.ToString().Trim(), out RemainingAmount)) {
                Fines F = new Fines();
                return ValidationResult.ValidResult;
            }
            return ValidationResult.ValidResult;
        }
    }
}
