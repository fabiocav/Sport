using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using nsoftware.InGoogle;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.ChallengeDateViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class ChallengeDateViewModel : BaseViewModel
	{
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