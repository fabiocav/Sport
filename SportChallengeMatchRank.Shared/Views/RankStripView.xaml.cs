using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public partial class RankStripView : ContentView
	{
		public static readonly BindableProperty MembershipProperty =
			BindableProperty.Create("Membership", typeof(Membership), typeof(RankStripView), null);

		public Membership Membership
		{
			get
			{
				return (Membership)GetValue(MembershipProperty);
			}
			set
			{
				SetValue(MembershipProperty, value);

				if(value != null)
				{
					UpperMembership = value.League.Memberships.SingleOrDefault(m => m.CurrentRank == value.CurrentRank - 1);
					LowerMembership = value.League.Memberships.SingleOrDefault(m => m.CurrentRank == value.CurrentRank + 1);
				}
				else
				{
					UpperMembership = null;
					LowerMembership = null;
				}
				OnPropertyChanged("UpperMembership");
				OnPropertyChanged("LowerMembership");
				OnPropertyChanged("DarkColor");
				OnPropertyChanged("LightColor");
			}
		}

		public Color DarkColor
		{
			get
			{
				return Membership == null || Membership.League == null ? Color.Transparent : Membership.League.Theme.Light.AddLuminosity(-.05);
			}
		}

		public Color LightColor
		{
			get
			{
				return Membership == null || Membership.League == null ? Color.Transparent : Membership.League.Theme.Light.AddLuminosity(.05);
			}
		}

		public Membership UpperMembership
		{
			get;
			private set;
		}

		public Membership LowerMembership
		{
			get;
			private set;
		}

		public Action<Membership> OnAthleteClicked
		{
			get;
			set;
		}

		public Action<Athlete> OnChallengeClicked
		{
			get;
			set;
		}

		public RankStripView()
		{
			InitializeComponent();
			root.BindingContext = this;
		}

		void HandleChallengeClicked(object sender, EventArgs e)
		{
			var btn = sender as Button;
			OnChallengeClicked?.Invoke((btn.BindingContext as Membership)?.Athlete);
		}

		void HandleAthleteClicked(object sender, EventArgs e)
		{
			var btn = sender as Button;
			OnAthleteClicked?.Invoke(btn.CommandParameter as Membership);
		}
	}
}