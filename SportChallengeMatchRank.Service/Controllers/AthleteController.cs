using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportChallengeMatchRank;
using SportChallengeMatchRank.Service.Models;

namespace SportChallengeMatchRank.Service.Controllers
{
    public class AthleteController : TableController<Athlete>
    {
		AppDataContext _context = new AppDataContext();
		
		protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new EntityDomainManager<Athlete>(_context, Request, Services);
        }

        // GET tables/Athlete
        public IQueryable<AthleteDto> GetAllAthletes()
        {
			return Query().Select(a => new AthleteDto
			{
				Name = a.Name,
				Id = a.Id,
				Email = a.Email,
				DateCreated = a.CreatedAt,
				IsAdmin = a.IsAdmin,
				AuthenticationId = a.AuthenticationId,
				MembershipIds = a.Memberships.Select(la => la.Id).ToList()
			});
        }

        // GET tables/Athlete/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<AthleteDto> GetAthlete(string id)
        {
			return SingleResult<AthleteDto>.Create(Lookup(id).Queryable.Select(a => new AthleteDto
			{
				Name = a.Name,
				Id = a.Id,
				DateCreated = a.CreatedAt,
				Email = a.Email,
				IsAdmin = a.IsAdmin,
				AuthenticationId = a.AuthenticationId,
				MembershipIds = a.Memberships.Select(la => la.Id).ToList()
			}));
        }

        // PATCH tables/Athlete/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Athlete> PatchAthlete(string id, Delta<Athlete> patch)
        {
           return UpdateAsync(id, patch);
        }

        // POST tables/Athlete
        public async Task<IHttpActionResult> PostAthlete(AthleteDto item)
        {
			var exists = _context.Athletes.Any(l => l.Email.Equals(item.Email, System.StringComparison.InvariantCultureIgnoreCase));

			if(exists)
				return Conflict();

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