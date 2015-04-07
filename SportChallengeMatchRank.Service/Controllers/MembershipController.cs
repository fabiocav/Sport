﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportChallengeMatchRank.Service.Models;
using System.Data.Entity;

namespace SportChallengeMatchRank.Service.Controllers
{
	//[AuthorizeLevel(AuthorizationLevel.User)]
	public class MembershipController : TableController<Membership>
    {
		AppDataContext _context = new AppDataContext();

		protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new EntityDomainManager<Membership>(_context, Request, Services);
        }

        // GET tables/Member
		public IQueryable<MembershipDto> GetAllMemberships()
        {
			return Query().Select(m => new MembershipDto
			{
				Id = m.Id,
				AthleteId = m.Athlete.Id,
				LeagueId = m.League.Id,
				IsAdmin = m.IsAdmin,
				CurrentRank = m.CurrentRank,
				DateCreated = m.CreatedAt,
			});
        }

        // GET tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public SingleResult<MembershipDto> GetMembership(string id)
        {
			return SingleResult<MembershipDto>.Create(Lookup(id).Queryable.Select(m => new MembershipDto
			{
				Id = m.Id,
				AthleteId = m.Athlete.Id,
				LeagueId = m.League.Id,
				IsAdmin = m.IsAdmin,
				CurrentRank = m.CurrentRank,
				DateCreated = m.CreatedAt,
			}));
		}

        // PATCH tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public Task<Membership> PatchMembership(string id, Delta<Membership> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Member
		public async Task<IHttpActionResult> PostMembership(MembershipDto item)
        {
			Membership current;
			var exists = _context.Memberships.FirstOrDefault(m => m.AthleteId == item.AthleteId && m.LeagueId == item.LeagueId);
			if(exists != null)
			{
				//Athlete is already a member of this league
				current = exists;
			}
			else
			{
				var prior = _context.Memberships.Where(m => m.LeagueId == item.LeagueId).OrderByDescending(m => m.CurrentRank).FirstOrDefault();

				if(prior != null)
					item.CurrentRank = prior.CurrentRank + 1;

				current = await InsertAsync(item.ToMembership());
			}

            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteMembership(string id)
        {
			var membership = _context.Memberships.SingleOrDefault(m => m.Id == id);

			//Need to remove all the existing challenges
			var challenges = _context.Challenges.Where(c => c.LeagueId == membership.LeagueId
				&& c.ChallengerAthleteId == membership.AthleteId
				|| c.ChallengeeAthleteId == membership.AthleteId).ToList();

			//Need to rerank the leaderboard
			var membershipsToAlter = _context.Memberships.Where(m => m.CurrentRank > membership.CurrentRank
				&& m.LeagueId == membership.LeagueId).ToList();
			membershipsToAlter.ForEach(m => m.CurrentRank -= 1);

			//TODO: send push notifications to opposing players letting them know challenge has been deleted

			challenges.ForEach(c => _context.Entry(c).State = EntityState.Deleted);
			_context.SaveChanges();

            return DeleteAsync(id);
        }
    }
}