using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportRankerMatchOn.Shared;
using SportRankerMatchOn.Service.Models;

namespace SportRankerMatchOn.Service.Controllers
{
    public class LeagueController : TableController<League>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            SportRankerMatchOnContext context = new SportRankerMatchOnContext();
            DomainManager = new EntityDomainManager<League>(context, Request, Services);
        }

        // GET tables/League
        public IQueryable<League> GetAllLeagues()
        {
            return Query();
        }

        // GET tables/League/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<League> GetLeague(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/League/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<League> PatchLeague(string id, Delta<League> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/League
        public async Task<IHttpActionResult> PostLeague(League item)
        {
            League current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/League/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteLeague(string id)
        {
            return DeleteAsync(id);
        }
    }
}