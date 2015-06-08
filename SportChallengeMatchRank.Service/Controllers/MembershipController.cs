using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportChallengeMatchRank.Service.Models;
using System.Data.Entity;
using System.Collections.Generic;
using System;
using SportChallengeMatchRank.Shared;

namespace SportChallengeMatchRank.Service.Controllers
{
	//[AuthorizeLevel(AuthorizationLevel.User)]
	public class MembershipController : TableController<Membership>
    {
		NotificationController _notificationController = new NotificationController();
		AppDataContext _context = new AppDataContext();

		protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new EntityDomainManager<Membership>(_context, Request, Services);
        }

        // GET tables/Member
		public IQueryable<MembershipDto> GetAllMemberships()
        {
			return ConvertMembershipToDto(Query());
        }

        // GET tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public SingleResult<MembershipDto> GetMembership(string id)
        {
			return SingleResult<MembershipDto>.Create(ConvertMembershipToDto(Lookup(id).Queryable));
		}

		IQueryable<MembershipDto> ConvertMembershipToDto(IQueryable<Membership> queryable)
		{
			return queryable.Select(m => new MembershipDto
			{
				Id = m.Id,
				UpdatedAt = m.UpdatedAt,
				AthleteId = m.Athlete.Id,
				LeagueId = m.League.Id,
				IsAdmin = m.IsAdmin,
				AbandonDate = m.AbandonDate,
				CurrentRank = m.CurrentRank,
				LastRankChange = m.LastRankChange,
				DateCreated = m.CreatedAt,
			});
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
			var exists = _context.Memberships.FirstOrDefault(m => m.AthleteId == item.AthleteId && m.LeagueId == item.LeagueId && m.AbandonDate == null);
			if(exists != null)
			{
				//Athlete is already a member of this league
				current = exists;
			}
			else
			{
				var prior = _context.Memberships.Where(m => m.LeagueId == item.LeagueId && m.AbandonDate == null)
					.OrderByDescending(m => m.CurrentRank).FirstOrDefault();

				if(prior != null)
					item.CurrentRank = prior.CurrentRank + 1;

				current = await InsertAsync(item.ToMembership());
			}

			try
			{
				var leagueName = _context.Leagues.Where(l => l.Id == item.LeagueId).Select(l => l.Name).ToList().First();
				var athleteName = _context.Athletes.Where(a => a.Id == item.AthleteId).Select(a => a.Name).ToList().First();
				var athleteAlias = _context.Athletes.Where(a => a.Id == item.AthleteId).Select(a => a.Alias).ToList().First();
				var message = "{0} (AKA {1}) hath joineth the {2} league".Fmt(athleteName, athleteAlias, leagueName);
				await _notificationController.NotifyByTag(message, item.LeagueId);
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
            
			return CreatedAtRoute("Tables", new { id = current.Id }, current);
		}

        // DELETE tables/Member/48D68C86-6EA6-4C25-AA33-223FC9A27959
        async public Task DeleteMembership(string id)
        {
			var membership = _context.Memberships.SingleOrDefault(m => m.Id == id);

			//Need to remove all the ongoing challenges (not past challenges since those should be locked and sealed in time for all to see for eternity)
			var challenges = _context.Challenges.Where(c => c.LeagueId == membership.LeagueId && c.DateCompleted == null
				&& (c.ChallengerAthleteId == membership.AthleteId || c.ChallengeeAthleteId == membership.AthleteId)).ToList();

			//Need to rerank the leaderboard
			var membershipsToAlter = _context.Memberships.Where(m => m.CurrentRank >= membership.CurrentRank
				&& m.LeagueId == membership.LeagueId && m.AbandonDate == null).ToList();
			membershipsToAlter.ForEach(m => m.CurrentRank -= 1);

			foreach(var c in challenges)
			{
				try
				{
					var league = _context.Leagues.SingleOrDefault(l => l.Id == c.LeagueId);
					var payload = new NotificationPayload
					{
						Action = PushActions.ChallengeDeclined,
						Payload = { { "leagueId", c.LeagueId }, { "challengeId", c.Id } }
					};
					var message = "You challenge with {0} has ben removed because they abandoned the {1} league".Fmt(membership.Athlete.Name, league.Name);
					await _notificationController.NotifyByTag(message, c.Opponent(membership.AthleteId).Id, payload);
				}
				catch(Exception e)
				{
					//TODO log to Insights
					Console.WriteLine(e);
				}
			}

			membership.AbandonDate = DateTime.UtcNow;
			challenges.ForEach(c => _context.Entry(c).State = EntityState.Deleted);
			_context.SaveChanges();
        }
    }
}