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

		public Style ButtonStyle
		{
			get;
			set;
		}

		public bool ShowChallengesButton
		{
			get
			{
				return false;
			}
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
		}

		void OnJoinClicked(object sender, EventArgs e)
		{
		}
	}
}