using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
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
}