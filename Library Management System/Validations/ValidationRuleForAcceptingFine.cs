using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Library_Management_System.Validations
{
    class ValidationRuleForAcceptingFine : ValidationRule
    {
        private int isseId;
        public int IsseId
        {
            get => isseId;
            set => isseId = value;
        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Fines F = new Fines();
            F.IssueId = isseId;
            double RAmount=0.0;
            RAmount = F.GetRamainingFineAmountOfSpecificUser();

            double EnteredAmount;
            if (double.TryParse(value.ToString(), out EnteredAmount))
            {
                if (EnteredAmount <= RAmount)
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    return new ValidationResult(false, "Maximum :- " + RAmount);
                }

            }
            else
            {
                return new ValidationResult(false, "Invalid Input");
            }
        }
    }
}
