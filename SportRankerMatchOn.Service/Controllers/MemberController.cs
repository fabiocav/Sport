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
    public class MemberController : TableController<Member>
    {
		SportRankerMatchOnContext _context = new SportRankerMatchOnContext();

		protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new EntityDomainManager<Member>(_context, Request, Services);
        }

        // GET tables/Member
		public IQueryable<MemberDto> GetAllMembers()
        {
			return Query().Select(m => new MemberDto
			{
				Id = m.Id,
				AthleteId = m.Athlete.Id,
				LeagueId = m.League.Id,
				CurrentRank = m.CurrentRank,
				JoinDate = m.CreatedAt,
			});
        }

        // GET tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public SingleResult<MemberDto> GetMember(string id)
        {
			return SingleResult<MemberDto>.Create(Lookup(id).Queryable.Select(m => new MemberDto
			{
				Id = m.Id,
				AthleteId = m.Athlete.Id,
				LeagueId = m.League.Id,
				CurrentRank = m.CurrentRank,
				JoinDate = m.CreatedAt,
			}));
		}

        // PATCH tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public Task<Member> PatchMember(string id, Delta<Member> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Member
		public async Task<IHttpActionResult> PostMember(MemberDto item)
        {
            Member current = await InsertAsync(item.ToMember());
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteMember(string id)
        {
            return DeleteAsync(id);
        }
    }
}