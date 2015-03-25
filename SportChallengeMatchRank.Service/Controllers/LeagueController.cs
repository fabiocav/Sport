using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportChallengeMatchRank;
using SportChallengeMatchRank.Service.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace SportChallengeMatchRank.Service.Controllers
{
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
				Sport = l.Sport,
				IsEnabled = l.IsEnabled,
				DateCreated = l.CreatedAt,
				CreatedByAthleteId = l.CreatedByAthlete.Id,
				ImageUrl = l.ImageUrl,
				Season = l.Season,
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
				Sport = l.Sport,
				IsEnabled = l.IsEnabled,
				DateCreated = l.CreatedAt,
				CreatedByAthleteId = l.CreatedByAthlete.Id,
				ImageUrl = l.ImageUrl,
				Season = l.Season,
				StartDate = l.StartDate,
				EndDate = l.EndDate,
				IsAcceptingMembers = l.IsAcceptingMembers,
				MembershipIds = l.Memberships.Select(m => m.Id).ToList()
			}));
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