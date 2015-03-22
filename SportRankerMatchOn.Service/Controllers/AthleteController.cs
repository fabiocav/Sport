using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportRankerMatchOn;
using SportRankerMatchOn.Service.Models;

namespace SportRankerMatchOn.Service.Controllers
{
    public class AthleteController : TableController<Athlete>
    {
		SportRankerMatchOnContext _context = new SportRankerMatchOnContext();
		
		protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new EntityDomainManager<Athlete>(_context, Request, Services);
        }

        // GET tables/Athlete
        public IQueryable<AthleteDto> GetAllAthletes()
        {
			return Query().Select(athlete => new AthleteDto
			{
				Name = athlete.Name,
				Id = athlete.Id,
				Email = athlete.Email,
				AuthenticationId = athlete.AuthenticationId,
				LeagueAssociationIds = athlete.LeagueAssociations.Select(la => la.Id).ToList()
			});
        }

        // GET tables/Athlete/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<AthleteDto> GetAthlete(string id)
        {
			return SingleResult<AthleteDto>.Create(Lookup(id).Queryable.Select(athlete => new AthleteDto
			{
				Name = athlete.Name,
				Id = athlete.Id,
				Email = athlete.Email,
				AuthenticationId = athlete.AuthenticationId,
				LeagueAssociationIds = athlete.LeagueAssociations.Select(la => la.Id).ToList()
			}));
        }

        // PATCH tables/Athlete/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<AthleteDto> PatchAthlete(string id, Delta<AthleteDto> patch)
        {
           // return UpdateAsync(id, patch);
			return null;
        }

        // POST tables/Athlete
        public async Task<IHttpActionResult> PostAthlete(AthleteDto item)
        {
            Athlete current = await InsertAsync(item.ToAthlete());
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Athlete/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteAthlete(string id)
        {
            return DeleteAsync(id);
        }
    }
}