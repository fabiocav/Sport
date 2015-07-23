using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;

namespace Sport.Shared
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
				LocalRefresh();
			}
		}

		public Color DarkColor
		{
			get
			{
				return Membership == null || Membership.League == null ? Color.Transparent : Membership.League.Theme.Light.AddLuminosity(-.1);
			}
		}

		public Color LightColor
		{
			get
			{
				return Membership == null || Membership.League == null ? Color.Transparent : Membership.League.Theme.Light.AddLuminosity(.07);
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

		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			base.LayoutChildren(x, y, width, height);
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

		void LocalRefresh()
		{
			if(Membership != null)
			{
				UpperMembership = Membership.League.Memberships.SingleOrDefault(m => m.CurrentRank == Membership.CurrentRank - 1);
				LowerMembership = Membership.League.Memberships.SingleOrDefault(m => m.CurrentRank == Membership.CurrentRank + 1);
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
}