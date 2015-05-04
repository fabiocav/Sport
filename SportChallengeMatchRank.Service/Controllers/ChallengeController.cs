using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportChallengeMatchRank.Service.Models;
using System.Collections.Generic;
using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using SportChallengeMatchRank.Shared;

namespace SportChallengeMatchRank.Service.Controllers
{
	//[AuthorizeLevel(AuthorizationLevel.User)]
	public class ChallengeController : TableController<Challenge>
    {
		NotificationController _notificationController = new NotificationController();
		AppDataContext _context = new AppDataContext();

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new EntityDomainManager<Challenge>(_context, Request, Services);
        }

        // GET tables/Challenge
        public IQueryable<ChallengeDto> GetAllChallenges()
        {
			return Query().Select(c => new ChallengeDto
			{
				Id = c.Id,
				ChallengerAthleteId = c.ChallengerAthleteId,
				ChallengeeAthleteId = c.ChallengeeAthleteId,
				LeagueId = c.LeagueId,
				DateCreated = c.CreatedAt,
				ProposedTime = c.ProposedTime,
				DateAccepted = c.DateAccepted,
				DateCompleted = c.DateCompleted,
				CustomMessage = c.CustomMessage,
				MatchResult = c.MatchResult.Select(r => new GameResultDto
				{
					Id = r.Id,
					DateCreated = r.CreatedAt,
					ChallengeId = r.ChallengeId,
					ChallengeeScore = r.ChallengeeScore,
					ChallengerScore = r.ChallengerScore,
					Index = r.Index
				}).ToList()
			});
        }

        // GET tables/Challenge/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<ChallengeDto> GetChallenge(string id)
        {
			return SingleResult<ChallengeDto>.Create(Lookup(id).Queryable.Select(dto => new ChallengeDto
			{
				Id = dto.Id,
				ChallengerAthleteId = dto.ChallengerAthleteId,
				ChallengeeAthleteId = dto.ChallengeeAthleteId,
				LeagueId = dto.LeagueId,
				DateCreated = dto.CreatedAt,
				ProposedTime = dto.ProposedTime,
				DateAccepted = dto.DateAccepted,
				DateCompleted = dto.DateCompleted,
				CustomMessage = dto.CustomMessage,
				MatchResult = dto.MatchResult.Select(r => new GameResultDto
				{
					Id = r.Id,
					DateCreated = r.CreatedAt,
					ChallengeId = r.ChallengeId,
					ChallengeeScore = r.ChallengeeScore,
					ChallengerScore = r.ChallengerScore,
					Index = r.Index
				}).ToList()
			}));
		}

        // PATCH tables/Challenge/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Challenge> PatchChallenge(string id, Delta<Challenge> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Challenge
        public async Task<IHttpActionResult> PostChallenge(ChallengeDto item)
        {
			//Check to see if there is already a challenge between the two athletes for this league
			var history = _context.Challenges.Where(c => ((c.ChallengeeAthleteId == item.ChallengeeAthleteId && c.ChallengerAthleteId == item.ChallengerAthleteId)
				|| (c.ChallengeeAthleteId == item.ChallengerAthleteId && c.ChallengerAthleteId == item.ChallengeeAthleteId))
				&& c.LeagueId == item.LeagueId).OrderByDescending(c => c.DateCompleted);

			//Check for ongoing challenges scheduled
			if(history.Any(c => c.DateCompleted == null))
				return Conflict();

			var league = _context.Leagues.SingleOrDefault(l => l.Id == item.LeagueId);
			var lastChallenge = history.FirstOrDefault();
			
			if(lastChallenge != null && lastChallenge.DateCompleted != null
				&& lastChallenge.ChallengerAthleteId == item.ChallengerAthleteId //is it the same athlete challenging again
				&& lastChallenge.GetChallengerWinningGames().Count() < lastChallenge.GetChallengeeWinningGames().Count() //did the challenger lose the previous match
				&& DateTime.UtcNow.Subtract(lastChallenge.DateCompleted.Value.UtcDateTime).TotalHours < league.MinHoursBetweenChallenge) //has enough time passed
			{
				return BadRequest(string.Format("Challenger must wait at least {0} hours before challenging again", league.MinHoursBetweenChallenge));
			}

			Challenge current = await InsertAsync(item.ToChallenge());
            var result = CreatedAtRoute("Tables", new { id = current.Id }, current.ToChallengeDto());

			var challenger = _context.Athletes.SingleOrDefault(a => a.Id == current.ChallengerAthleteId);
			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == current.ChallengeeAthleteId);

			var message = "YOU HAVE BEEN CHALLENGED by {0}!".Fmt(challenger.Name);
			await _notificationController.NotifyByTag(message, current.ChallengeeAthleteId);

			return result;
        }

		[HttpGet]
		[Route("api/revokeChallenge")]
		public async Task RevokeChallenge(string id)
        {
			var challenge = Lookup(id).Queryable.FirstOrDefault();
			var challenger = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengerAthleteId);
			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);

			var message = "Your challenge with {0} has been revoked.".Fmt(challenger.Name);
			await _notificationController.NotifyByTag(message, challenge.ChallengeeAthleteId);

            DeleteAsync(id);
        }

		[HttpGet]
		[Route("api/declineChallenge")]
		public async Task DeclineChallenge(string id)
		{
			var challenge = Lookup(id).Queryable.FirstOrDefault();
			var challenger = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengerAthleteId);
			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);

			var message = "Your challenge with {0} has been declined.".Fmt(challengee.Name);
			await _notificationController.NotifyByTag(message, challenge.ChallengerAthleteId);

			DeleteAsync(id);
		}

		[Route("api/getChallengesForAthlete")]
		async public Task<List<ChallengeDto>> GetChallengesForAthlete(string athleteId)
		{
			return Query().Where(c => c.ChallengeeAthleteId == athleteId
				|| c.ChallengerAthleteId == athleteId).Select(c => new ChallengeDto
			{
				Id = c.Id,
				ChallengerAthleteId = c.ChallengerAthleteId,
				ChallengeeAthleteId = c.ChallengeeAthleteId,
				LeagueId = c.LeagueId,
				DateCreated = c.CreatedAt,
				ProposedTime = c.ProposedTime,
				DateAccepted = c.DateAccepted,
				DateCompleted = c.DateCompleted,
				CustomMessage = c.CustomMessage,
				MatchResult = c.MatchResult.Select(r => new GameResultDto
				{
					Id = r.Id,
					DateCreated = r.CreatedAt,
					ChallengeId = r.ChallengeId,
					ChallengeeScore = r.ChallengeeScore,
					ChallengerScore = r.ChallengerScore,
					Index = r.Index
				}).ToList()
			}).ToList();
		}

		[Route("api/acceptChallenge")]
		async public Task<ChallengeDto> AcceptChallenge(string challengeId)
		{
			var challenge = _context.Challenges.SingleOrDefault(c => c.Id == challengeId);
			challenge.DateAccepted = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);
			var message = "Your challenge with {0} has been accepted! MATCH ON!!".Fmt(challengee.Name);
			await _notificationController.NotifyByTag(message, challenge.ChallengerAthleteId);

			return challenge.ToChallengeDto();
		}

		[Route("api/postMatchResults")]
		async public Task<ChallengeDto> PostMatchResults(List<GameResultDto> results)
		{
			if(results.Count < 1)
			{
				//BadRequest
				return null;
			}

			var challengeId = results.First().ChallengeId;
			var challenge = _context.Challenges.SingleOrDefault(c => c.Id == challengeId);

			if(challenge.DateCompleted != null)
			{
				//BadRequest
				return null;
			}

			var league = _context.Leagues.SingleOrDefault(l => l.Id == challenge.LeagueId);

			if(league == null || results.Count != league.MatchGameCount)
				throw new Exception("Result count not equal league match game count");

			challenge.DateCompleted = DateTime.UtcNow;
			var dto = challenge.ToChallengeDto();
			dto.MatchResult = new List<GameResultDto>();

			foreach(var result in results)
			{
				result.Id = Guid.NewGuid().ToString();
				_context.GameResults.Add(result.ToGameResult());
				dto.MatchResult.Add(result);
			}

			try
			{
				_context.SaveChanges();

				var challengerWins = challenge.GetChallengerWinningGames();
				var challengeeWins = challenge.GetChallengeeWinningGames();
				var challenger = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengerAthleteId);
				var challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);
				var challengerMembership = _context.Memberships.SingleOrDefault(m => m.AthleteId == challenger.Id && m.LeagueId == challenge.LeagueId);
				var challengeeMembership = _context.Memberships.SingleOrDefault(m => m.AthleteId == challengee.Id && m.LeagueId == challenge.LeagueId);
				var winningRank = challengeeMembership.CurrentRank;

				if(challengerWins.Length > challengeeWins.Length)
				{
					challenger = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengerAthleteId);
					challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);

					var oldRank = challengerMembership.CurrentRank;
					challengerMembership.CurrentRank = challengeeMembership.CurrentRank;
					challengeeMembership.CurrentRank = oldRank;
					winningRank = challengerMembership.CurrentRank;

					_context.SaveChanges();
				}

				if(challengeeWins.Length > challengerWins.Length)
				{
					challenger = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);
					challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengerAthleteId);
				}

				var maintain = challenge.ChallengerAthlete.Id == challenger.Id ? "bequeath" : "retain";
				var message = "{0} victors over {1} to {2} the righteous rank of {3}".Fmt(challenger.Alias, challengee.Alias, maintain, winningRank);
				await _notificationController.NotifyByTag(message, challenge.LeagueId);
			}
			catch(DbEntityValidationException e)
			{
				#region Error Print

				foreach(var eve in e.EntityValidationErrors)
				{
					Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
						eve.Entry.Entity.GetType().Name, eve.Entry.State);
					foreach(var ve in eve.ValidationErrors)
					{
						Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
							ve.PropertyName, ve.ErrorMessage);
					}
				}
				throw;

				#endregion
			}

			return dto;
		}
	}
}