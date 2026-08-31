using System.Globalization;

namespace MauiApp1.Converters
{
    /*Sa profile to erp sa masterlist yung initials*/
    public class InitialsConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string name || string.IsNullOrWhiteSpace(name))
                return "?";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return "?";

            if (parts.Length == 1)
                return parts[0][0].ToString().ToUpper(culture);

            return string.Concat(parts[0][0], parts[^1][0]).ToUpper(culture);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
