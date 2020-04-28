using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Library_Management_System.Validations
{
    public class RowConverterForPendingIssuedBook : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double TotalDays;
            string Val = value.ToString();
            DateTime IDate;
            DateTime.TryParse(Val ,out IDate);
            TotalDays = (DateTime.Today - IDate).TotalDays;
            if (TotalDays > 10 && TotalDays < 20)
            {
                return Brushes.Yellow;
            }
            else if (TotalDays > 20 && TotalDays < 30)
            {
                return Brushes.Orange;
            }
            else if (TotalDays > 30) {
                return Brushes.Red;
            }
            else
            {
                return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
