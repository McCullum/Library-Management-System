using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace Library_Management_System.Validations
{
    public class UserIdExistsOrNotValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Users U = new Users();
            int UserId;
            if (int.TryParse(value.ToString(),out UserId)) {
                U.UserId = UserId;
                if (U.CheckUserExistsOrNot())
                {
                    return ValidationResult.ValidResult;
                }
                else {
                    return new ValidationResult(false, "User Does Not Exists!");
                }

            }
            else{
                return new ValidationResult(false, "User Id Must Be Digit!");
            }
        }
    }
}
