using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System.Collections.Generic;

[assembly: Dependency(typeof(Sport.Shared.ChallengeDateViewModel))]

namespace Sport.Shared
{
	public class ChallengeDateViewModel : BaseViewModel
	{
		public Athlete Opponent
		{
			get
			{
				return Challenge != null && Challenge.ChallengeeAthleteId != App.CurrentAthlete.Id ? Challenge?.ChallengeeAthlete : Challenge?.ChallengerAthlete;
			}
		}

		Challenge _challenge;

		public Challenge Challenge
		{
			get
			{
				return _challenge;
			}
			set
			{
				SetPropertyChanged(ref _challenge, value);
			}
		}

		public DateTime SelectedDateTime
		{
			get
			{
				return SelectedDate.Add(SelectedTime);
			}
		}

		DateTime _selectedDate;

		public DateTime SelectedDate
		{
			get
			{
				return _selectedDate;
			}
			set
			{
				SetPropertyChanged(ref _selectedDate, value);
				SetPropertyChanged("SelectedDateTime");
			}
		}

		TimeSpan _selectedTime;

		public TimeSpan SelectedTime
		{
			get
			{
				return _selectedTime;
			}
			set
			{
				SetPropertyChanged(ref _selectedTime, value);
				SetPropertyChanged("SelectedDateTime");
			}
		}

		async public Task<Challenge> PostChallenge()
		{
			Challenge.ProposedTime = SelectedDateTime.ToUniversalTime();				
			var task = AzureService.Instance.SaveChallenge(Challenge);
			await RunSafe(task);

			if(task.IsFaulted)
				return null;

			task = AddChallengeEventToCalendar();
			await RunSafe(task);

			Challenge.League.RefreshChallenges();
			MessagingCenter.Send<App>(App.Current, "ChallengesUpdated");
			return Challenge;
		}

		public void CreateChallenge(Athlete challenger, Athlete challengee, League league)
		{
			var time = TimeSpan.FromTicks(DateTime.Now.AddMinutes(60).Subtract(DateTime.Today).Ticks);

			if(time.Ticks > TimeSpan.TicksPerDay)
				time = time.Subtract(TimeSpan.FromTicks(TimeSpan.TicksPerDay));

			SelectedTime = time;
			SelectedDate = DateTime.Today;

			var membership = league.Memberships.SingleOrDefault(m => m.AthleteId == challengee.Id);

			Challenge = new Challenge {
				BattleForRank = membership.CurrentRank,
				ChallengerAthleteId = challenger.Id,
				ChallengeeAthleteId = challengee.Id,
				ProposedTime = SelectedDateTime,
				LeagueId = league.Id,
			};
		}

		public Task AddChallengeEventToCalendar()
		{
			return new Task(() =>
			{
				var service = new CalendarService();
				service.HttpClient.DefaultRequestHeaders.Add("Authorization", Settings.Instance.AuthToken);
				var req = service.CalendarList.List();
				var list = req.Execute();

				var primaryCalendar = list.Items.ToList().FirstOrDefault(i => i.Primary.HasValue && i.Primary.Value);

				if(primaryCalendar == null)
				{
					"Unable to locate default calendar".ToToast();
					return;
				}

				var evnt = new Event();
				evnt.Attendees = new List<EventAttendee> {
					new EventAttendee {
						Email = Opponent.Email,
						DisplayName = Opponent.Name,
					}
				};

				evnt.Summary = "{0}: {1} vs {2}".Fmt(Challenge.League.Name, Challenge.ChallengerAthlete.Alias, Challenge.ChallengeeAthlete.Alias);
				evnt.Description = Challenge.BattleForPlaceBetween;
				evnt.Start = new EventDateTime {
					DateTime = Challenge.ProposedTime.UtcDateTime,
					TimeZone = "GMT",
				};

				evnt.End = new EventDateTime {
					DateTime = Challenge.ProposedTime.UtcDateTime.AddMinutes(30),
					TimeZone = "GMT",
				};

				var saved = service.Events.Insert(evnt, primaryCalendar.Id).Execute();
				Console.WriteLine(saved.HtmlLink);
			});
		}


		public string Validate()
		{
			var sb = new StringBuilder();

			if(SelectedDate.AddTicks(SelectedTime.Ticks) <= DateTime.Now.AddMinutes(5))
			{
				sb.AppendLine("Please choose a date at least 5 minutes from now.");
			}

			return sb.Length == 0 ? null : sb.ToString();
		}
	}
}