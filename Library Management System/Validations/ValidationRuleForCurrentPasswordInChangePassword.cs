using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace Library_Management_System.Validations
{
    public class ValidationRuleForCurrentPasswordInChangePassword : ValidationRule
    {
        private int userID;

        public int UserID {
            get => userID;
            set => userID = value;
        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Users U = new Users();
            U.UserId = userID;
            if (U.CheckCurrentPassword(value.ToString()))
            {
                return ValidationResult.ValidResult;
            }
            else {
                return new ValidationResult(false, "This Is Not Your Current Password");
            }
        }
    }
}
