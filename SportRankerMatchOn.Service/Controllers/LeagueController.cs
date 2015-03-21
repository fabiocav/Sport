using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportRankerMatchOn;
using SportRankerMatchOn.Service.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportRankerMatchOn.Service.Controllers
{
    public class LeagueController : TableController<League>
    {
		SportRankerMatchOnContext _context = new SportRankerMatchOnContext();

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
				Season = l.Season,
				IsAcceptingMembers = l.IsAcceptingMembers,
				MemberIds = l.Members.Select(m => m.Id).ToList()
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
				Season = l.Season,
				IsAcceptingMembers = l.IsAcceptingMembers,
				MemberIds = l.Members.Select(m => m.Id).ToList()
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