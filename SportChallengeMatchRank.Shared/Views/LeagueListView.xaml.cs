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
	}

	public class CustomViewCell : ViewCell
	{
		static int _index = 0;

		public CustomViewCell()
		{
			_index++;
		}

		protected override void OnChildAdded(Element child)
		{
			child.StyleId = "custom_cell_" + _index;
			base.OnChildAdded(child);
		}
	}
}