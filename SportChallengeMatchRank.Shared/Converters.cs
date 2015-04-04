using System;
using Xamarin.Forms;
using System.Globalization;
using System.Collections;

namespace SportChallengeMatchRank.Shared
{
	public class InverseBoolConverter : IValueConverter
	{
		public static InverseBoolConverter Instance = new InverseBoolConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}
	}

	public class IsNotNullToBoolConverter : IValueConverter
	{
		public static IsNotNullToBoolConverter Instance = new IsNotNullToBoolConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null;
		}
	}

	public class IsNullToBoolConverter : IValueConverter
	{
		public static IsNullToBoolConverter Instance = new IsNullToBoolConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null;
		}
	}

	public class IsEmptyConverter : IValueConverter
	{
		public static IsEmptyConverter Instance = new IsEmptyConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var list = value as IList;

			if(list == null)
				return false;

			return list.Count > 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var list = value as IList;

			if(list == null)
				return false;

			return list.Count > 0;
		}
	}
}