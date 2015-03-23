﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportRankerMatchOn;
using SportRankerMatchOn.Service.Models;

namespace SportRankerMatchOn.Service.Controllers
{
    public class MembershipController : TableController<Membership>
    {
		SportRankerMatchOnContext _context = new SportRankerMatchOnContext();

		protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new EntityDomainManager<Membership>(_context, Request, Services);
        }

        // GET tables/Member
		public IQueryable<MembershipDto> GetAllMembers()
        {
			return Query().Select(m => new MembershipDto
			{
				Id = m.Id,
				AthleteId = m.Athlete.Id,
				LeagueId = m.League.Id,
				IsAdmin = m.IsAdmin,
				CurrentRank = m.CurrentRank,
				JoinDate = m.CreatedAt,
			});
        }

        // GET tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public SingleResult<MembershipDto> GetMember(string id)
        {
			return SingleResult<MembershipDto>.Create(Lookup(id).Queryable.Select(m => new MembershipDto
			{
				Id = m.Id,
				AthleteId = m.Athlete.Id,
				LeagueId = m.League.Id,
				IsAdmin = m.IsAdmin,
				CurrentRank = m.CurrentRank,
				JoinDate = m.CreatedAt,
			}));
		}

        // PATCH tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public Task<Membership> PatchMember(string id, Delta<Membership> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Member
		public async Task<IHttpActionResult> PostMember(MembershipDto item)
        {
            Membership current = await InsertAsync(item.ToMember());
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteMember(string id)
        {
            return DeleteAsync(id);
        }
    }
}