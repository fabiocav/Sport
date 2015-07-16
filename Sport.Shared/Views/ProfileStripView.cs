using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;

namespace Sport.Shared
{
	public partial class ProfileStripView : ContentView
	{
		public static readonly BindableProperty AthleteProperty =
			BindableProperty.Create("Athlete", typeof(Athlete), typeof(ProfileStripView), null);

		public Athlete Athlete
		{
			get
			{
				return (Athlete)GetValue(AthleteProperty);
			}
			set
			{
				SetValue(AthleteProperty, value);
			}
		}

		public static readonly BindableProperty DarkColorProperty =
			BindableProperty.Create("DarkColor", typeof(Color), typeof(ProfileStripView), Color.Gray);

		public Color DarkColor
		{
			get
			{
				return (Color)GetValue(DarkColorProperty);
			}
			set
			{
				SetValue(DarkColorProperty, value);
			}
		}

		public ProfileStripView()
		{
			InitializeComponent();
			root.BindingContext = this;
		}
	}
}