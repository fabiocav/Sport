using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportChallengeMatchRank.Service.Models;
using System;

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
			_notificationController.NotifyByTag(message, league.Id);

			return league.StartDate.Value.UtcDateTime;
		}

        // PATCH tables/League/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<League> PatchLeague(string id, Delta<League> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/League
        public async Task<IHttpActionResult> PostLeague(LeagueDto item)
        {
			var exists = _context.Leagues.Any(l => l.Name.Equals(item.Name, System.StringComparison.InvariantCultureIgnoreCase));

			if(exists)
				return BadRequest("The name of that league is already in use.");

			League current = await InsertAsync(item.ToLeague());
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/League/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteLeague(string id)
        {
            return DeleteAsync(id);
        }
    }
}