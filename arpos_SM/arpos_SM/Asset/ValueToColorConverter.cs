using System;
using Xamarin.Forms;

namespace arpos_SM.Asset
{
    public class ValueToColorConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var strVal = (int)value;

            switch (strVal)
            {
                case 1:
                    return Color.Gray;
                case 2:
                    return Color.Maroon;
                case 3:
                    return Color.Red;
                default:
                    return Color.Black;
                    //throw new NotSupportedException("Unknown Value");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
