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
			}
		}

		TimeSpan _selectedTime = TimeSpan.FromHours(DateTime.UtcNow.ToLocalTime().Hour + 3);

		public TimeSpan SelectedTime
		{
			get
			{
				return _selectedTime;
			}
			set
			{
				SetPropertyChanged(ref _selectedTime, value);
			}
		}

		async public Task<Challenge> ChallengeAthlete(Athlete challenger, Athlete challengee, League league)
		{
			var challenge = new Challenge {
				ChallengerAthleteId = challenger.Id,
				ChallengeeAthleteId = challengee.Id,
				ProposedTime = SelectedDate.AddTicks(SelectedTime.Ticks),
				LeagueId = league.Id,
			};

			var task = AzureService.Instance.SaveChallenge(challenge);
			await RunSafe(task);

			if(task.IsFaulted)
				return null;

			challenger.RefreshChallenges();
			challenger.RefreshChallenges();
			return challenge;
		}

		public string Validate()
		{
			var sb = new StringBuilder();

			if(SelectedDate.AddTicks(SelectedTime.Ticks) <= DateTime.Now.AddMinutes(30))
			{
				sb.AppendLine("Please choose a date at least 30 minutes from now.");
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