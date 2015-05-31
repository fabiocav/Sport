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
			get;
			set;
		} = false;

		void OnRankingsClicked(object sender, EventArgs e)
		{
		}
	}
}