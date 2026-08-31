using System.Globalization;

namespace MauiApp1.Converters
{
    /*Basta sa display lang to ng oras ginagawa nyang -- pag empty pa or null pa time in or time out*/
    public class TimeDisplayConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                if (dt == default)
                    return "--";

                return dt.ToString("h:mm tt", culture);
            }

            return "--";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /* eto yung naghahandle ng status erp kung nakapag in out or in progress pa*/
    public class AttendanceStatusTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dt && dt != default)
                return "Complete";

            return "In Progress";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    /*kulay lang to kung successful or hindi yung sca*/
    public class AttendanceStatusColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dt && dt != default)
                return Color.FromArgb("#27AE60"); // green

            return Color.FromArgb("#F59E0B"); // amber
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
