using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class ChallengeCardView : ContentView
	{
		public static readonly BindableProperty ChallengeProperty =
			BindableProperty.Create("Challenge", typeof(Challenge), typeof(ChallengeCardView), null);

		public Challenge Challenge
		{
			get
			{
				return (Challenge)GetValue(ChallengeProperty);
			}
			set
			{
				SetValue(ChallengeProperty, value);
			}
		}

		public ChallengeCardView()
		{
			InitializeComponent();
			root.BindingContext = this;
		}

		void HandleClick(object sender, EventArgs e)
		{
			
		}
	}
}