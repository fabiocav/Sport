using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using nsoftware.InGoogle;
using System.Text;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.ChallengeDateViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class ChallengeDateViewModel : BaseViewModel
	{
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

		DateTime _selectedDate = DateTime.Today;

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

			Challenge.League.RefreshChallenges();
			return Challenge;
		}

		public void CreateChallenge(Athlete challenger, Athlete challengee, League league)
		{
			SelectedTime = TimeSpan.FromTicks(DateTime.Now.AddMinutes(60).Subtract(DateTime.Today).Ticks);

			if(SelectedTime.Ticks > TimeSpan.TicksPerDay)
				SelectedTime = SelectedTime.Subtract(TimeSpan.FromTicks(TimeSpan.TicksPerDay));

			Challenge = new Challenge {
				ChallengerAthleteId = challenger.Id,
				ChallengeeAthleteId = challengee.Id,
				ProposedTime = SelectedDateTime,
				LeagueId = league.Id,
			};
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

		public async Task CrossReferenceCalendars(Athlete a, Athlete b, DateTime date)
		{
			using(new Busy(this))
			{
				var calendar = await GetCalendarForAthlete(a);
			}
		}

		async Task<string> GetCalendarForAthlete(Athlete athlete)
		{
			try
			{
				Gcalendar cal = new Gcalendar();
				cal.Authorization = Settings.Instance.AuthToken;

				await cal.ListCalendarsAsync();



				for(int i = 0; i < cal.CalendarCount; i++)
				{
					cal.CalendarIndex = i;

					if(cal.CalendarPrimary)
					{
						Console.WriteLine(cal.CalendarSummary);

						cal.EventStartDate.DateTime = DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToString();
						cal.EventEndDate.DateTime = DateTime.Now.Add(TimeSpan.FromDays(1)).ToString();
						await cal.ListEventsAsync();

						for(int j = 0; j < cal.EventCount; j++)
						{
							cal.EventIndex = j;
							Console.WriteLine(cal.EventStartDate.DateTime + " " + cal.EventSummary);
						}
					}
				}
				return null;

			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
			return null;
		}
	}
}