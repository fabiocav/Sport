using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;

[assembly: Dependency(typeof(SportRankerMatchOn.Shared.MembershipsByAthleteViewModel))]
namespace SportRankerMatchOn.Shared
{
	public class MembershipsByAthleteViewModel : BaseViewModel
	{
		Athlete _athlete;
		public const string AthletePropertyName = "Athlete";

		public Athlete Athlete
		{
			get
			{
				return _athlete;
			}
			set
			{
				SetProperty(ref _athlete, value, AthletePropertyName);
			}
		}

		public ICommand GetAllMembershipsByAthleteCommand
		{
			get
			{
				return new Command(async() => await GetAllMembershipsByAthlete(true));
			}
		}

		async public Task GetAllMembershipsByAthlete(bool forceRefresh = false)
		{
			if(!forceRefresh && Athlete.Memberships.Count > 0)
				return;
			
			using(new Busy(this))
			{
				await AzureService.Instance.GetAllLeaguesByAthlete(Athlete);
			}
		}
	}
}