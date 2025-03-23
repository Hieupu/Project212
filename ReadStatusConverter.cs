using System;
using System.Globalization;
using System.Windows.Data;

namespace Project212
{
    public class ReadStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRead)
            {
                return isRead ? "Đã đọc" : "Thông báo mới";
            }
            return "Không xác định";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "Đã đọc";
        }

    }
}
