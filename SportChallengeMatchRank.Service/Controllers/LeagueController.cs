using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportChallengeMatchRank.Service.Models;
using System;
using SportChallengeMatchRank.Shared;

namespace SportChallengeMatchRank.Service.Controllers
{
	//[AuthorizeLevel(AuthorizationLevel.User)]
	public class LeagueController : TableController<League>
    {
		NotificationController _notificationController = new NotificationController();
		AppDataContext _context = new AppDataContext();

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new EntityDomainManager<League>(_context, Request, Services);
        }

        // GET tables/League
        public IQueryable<LeagueDto> GetAllLeagues()
        {
			return Query().Select(l => new LeagueDto
			{
				Id = l.Id,
				Name = l.Name,
				Description = l.Description,
				Sport = l.Sport,
				IsEnabled = l.IsEnabled,
				DateCreated = l.CreatedAt,
				CreatedByAthleteId = l.CreatedByAthlete.Id,
				ImageUrl = l.ImageUrl,
				Season = l.Season,
				MaxChallengeRange = l.MaxChallengeRange,
				MinHoursBetweenChallenge = l.MinHoursBetweenChallenge,
				MatchGameCount = l.MatchGameCount,
				HasStarted = l.HasStarted,
				StartDate = l.StartDate,
				EndDate = l.EndDate,
				IsAcceptingMembers = l.IsAcceptingMembers,
				MembershipIds = l.Memberships.Select(m => m.Id).ToList()
			});
        }

        // GET tables/League/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<LeagueDto> GetLeague(string id)
        {
			return SingleResult<LeagueDto>.Create(Lookup(id).Queryable.Select(l => new LeagueDto
			{
				Id = l.Id,
				Name = l.Name,
				Description = l.Description,
				Sport = l.Sport,
				IsEnabled = l.IsEnabled,
				DateCreated = l.CreatedAt,
				CreatedByAthleteId = l.CreatedByAthlete.Id,
				ImageUrl = l.ImageUrl,
				Season = l.Season,
				MaxChallengeRange = l.MaxChallengeRange,
				MinHoursBetweenChallenge = l.MinHoursBetweenChallenge,
				MatchGameCount = l.MatchGameCount,
				HasStarted = l.HasStarted,
				StartDate = l.StartDate,
				EndDate = l.EndDate,
				IsAcceptingMembers = l.IsAcceptingMembers,
				MembershipIds = l.Memberships.Select(m => m.Id).ToList()
			}));
		}

		[Route("api/startLeague")]
		async public Task<DateTime> StartLeague(string id)
		{
			var league = _context.Leagues.SingleOrDefault(l => l.Id == id);
			league.HasStarted = true;
			league.StartDate = DateTime.Now.ToUniversalTime();

			var memberships = _context.Memberships.Where(m => m.LeagueId == id).ToList();

			if(memberships.Count < 2)
			{
				//TODO Enable this validation
				//return Conflict("Must have at least 2 members before starting a league.");
			}
	
			memberships.Shuffle();

			//Randomize the athlete rankage when the league kicks off
			var i = 0;
			foreach(var m in memberships)
			{
				m.CurrentRank = i + 1;
				i++;
			}
			_context.SaveChanges();

			var message = "The {0} league has officially started. It's on like a prawn that yawns at dawn!".Fmt(league.Name);
			var payload = new NotificationPayload
			{
				Action = PushActions.LeagueStarted,
				Payload = { { "leagueId", id } }
			};
			_notificationController.NotifyByTag(message, league.Id, payload);

			return league.StartDate.Value.UtcDateTime;
		}

        // PATCH tables/League/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<League> PatchLeague(string id, Delta<League> patch)
        {
			var league = _context.Leagues.SingleOrDefault(l => l.Id == id);

			var updated = patch.GetEntity();
			if(!league.IsAcceptingMembers && updated.IsAcceptingMembers)
			{
				NotifyAboutNewLeagueOpenEnrollment(updated);
			}

            return UpdateAsync(id, patch);
        }

		void NotifyAboutNewLeagueOpenEnrollment(League league)
		{
			var date = league.StartDate.Value.DateTime.ToOrdinal();
			var message = "The {0} league has been created and will begin on {1}.  Be cool for once and join!".Fmt(league.Name, date);
			var payload = new NotificationPayload
			{
				Action = PushActions.LeagueStarted,
				Payload = { { "leagueId", league.Id } }
			};
			_notificationController.NotifyByTag(message, "All", payload);
		}

        // POST tables/League
        public async Task<IHttpActionResult> PostLeague(LeagueDto item)
        {
			var exists = _context.Leagues.Any(l => l.Name.Equals(item.Name, System.StringComparison.InvariantCultureIgnoreCase));

			if(exists)
				return BadRequest("The name of that league is already in use.");

			League league = await InsertAsync(item.ToLeague());

			if(league.IsAcceptingMembers)
			{
				NotifyAboutNewLeagueOpenEnrollment(league);
			}

            return CreatedAtRoute("Tables", new { id = league.Id }, league);
        }

        // DELETE tables/League/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteLeague(string id)
        {
			var league = _context.Leagues.SingleOrDefault(l => l.Id == id);
			var message = "The {0} league has been removed.".Fmt(league.Name);
			var payload = new NotificationPayload
			{
				Action = PushActions.LeagueEnded,
				Payload = { { "leagueId", id } }
			};
			_notificationController.NotifyByTag(message, league.Id, payload);

            return DeleteAsync(id);
        }
    }
}