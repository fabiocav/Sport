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
			KickoffStartLeagueNotifications(league.Name, league.Id);
			return league.StartDate.Value.UtcDateTime;
		}

		async Task KickoffStartLeagueNotifications(string leagueName, string leagueId)
		{
			try
			{
				//var dict = new Dictionary<string, object> { { "leagueId", leagueId } };

				//var message = AppDataContext.GetPush("The {0} league has started! Challenge away :)".Fmt(leagueName), new { { "leagueId", leagueId } });
				//message.Add();
				//var pushResult = await Services.Push.SendAsync(message, leagueId);
				//Services.Log.Info(pushResult.State.ToString());
			}
			catch(System.Exception ex)
			{
				Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
			}
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
				return Conflict();

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