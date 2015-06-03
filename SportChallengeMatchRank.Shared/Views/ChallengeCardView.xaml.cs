using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class ChallengeCardView : ContentView
	{
		public static readonly BindableProperty ViewModelProperty =
			BindableProperty.Create("ViewModel", typeof(ChallengeDetailsViewModel), typeof(ChallengeCardView), null);

		public ChallengeDetailsViewModel ViewModel
		{
			get
			{
				return (ChallengeDetailsViewModel)GetValue(ViewModelProperty);
			}
			set
			{
				SetValue(ViewModelProperty, value);
			}
		}

		public Action OnClicked
		{
			get;
			set;
		}

		public Action OnPostResults
		{
			get;
			set;
		}

		public Action OnAccepted
		{
			get;
			set;
		}

		public Action OnDeclined
		{
			get;
			set;
		}

		public ChallengeCardView()
		{
			InitializeComponent();
			root.BindingContext = this;

			root.GestureRecognizers.Add(new TapGestureRecognizer((view) =>
			{
				OnClicked?.Invoke();
			}));
		}

		void HandlePostResults(object sender, EventArgs e)
		{
			OnPostResults?.Invoke();
		}

		void HandleDeclined(object sender, EventArgs e)
		{
			OnDeclined?.Invoke();
		}

		void HandleAccepted(object sender, EventArgs e)
		{
			OnAccepted?.Invoke();
		}
	}
}