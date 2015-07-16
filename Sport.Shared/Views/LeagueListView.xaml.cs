using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Globalization;

namespace Sport.Shared
{
	public partial class LeagueListView : ListView
	{
		public LeagueListView()
		{
			InitializeComponent();
		}

		public bool ShowChallengesButton
		{
			get
			{
				return false;
			}
		}

		public Action<League> OnRankings
		{
			get;
			set;
		}

		public Action<League> OnJoin
		{
			get;
			set;
		}

		public bool ShowJoinButton
		{
			get;
			set;
		} = false;

		public bool ShowRankingsButton
		{
			get;
			set;
		} = false;

		void OnRankingsClicked(object sender, EventArgs e)
		{
			var league = ((Button)sender).CommandParameter as League;
			OnRankings?.Invoke(league);
		}

		void OnJoinClicked(object sender, EventArgs e)
		{
			var league = ((Button)sender).CommandParameter as League;
			OnJoin?.Invoke(league);
		}
	}

	public class LeaguePaddingValueConverter : IValueConverter
	{
		public static LeaguePaddingValueConverter Instance = new LeaguePaddingValueConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isLast = (bool)value;
			return isLast ? new Thickness(14) : new Thickness(14, 14, 14, 0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}