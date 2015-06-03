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
using System.Net;
using System.Net.Http;

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
				UpdatedAt = c.UpdatedAt,
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
				UpdatedAt = dto.UpdatedAt,
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
					UpdatedAt = c.UpdatedAt,
					DateCompleted = c.DateCompleted,
					CustomMessage = c.CustomMessage,
					MatchResult = c.MatchResult.Where(r => r.ChallengeeScore != null && r.ChallengerScore != null)
						.OrderBy(r => r.Index).Select(r => new GameResultDto
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
		
		// PATCH tables/Challenge/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Challenge> PatchChallenge(string id, Delta<Challenge> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Challenge
        public async Task<IHttpActionResult> PostChallenge(ChallengeDto item)
        {
			var challenger = _context.Athletes.SingleOrDefault(a => a.Id == item.ChallengerAthleteId);
			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == item.ChallengeeAthleteId);

			//Check to see if there are any ongoing challenges between either athlete
			var challengeeOngoing = _context.Challenges.Where(c => (c.ChallengeeAthleteId == item.ChallengeeAthleteId || c.ChallengeeAthleteId == item.ChallengerAthleteId)
				&& c.LeagueId == item.LeagueId && c.DateCompleted == null);

			if(challengeeOngoing.Count() > 0)
				throw "{0} already has an existing challenge underway.".Fmt(challengee.Alias).ToException(Request);

			var challengerOngoing = _context.Challenges.Where(c => (c.ChallengerAthleteId == item.ChallengeeAthleteId || c.ChallengerAthleteId == item.ChallengerAthleteId)
				&& c.LeagueId == item.LeagueId && c.DateCompleted == null);

			if(challengerOngoing.Count() > 0)
				throw "You already have an existing challenge underway.".ToException(Request);

			//Check to see if there is already a challenge between the two athletes for this league
			var history = _context.Challenges.Where(c => ((c.ChallengeeAthleteId == item.ChallengeeAthleteId && c.ChallengerAthleteId == item.ChallengerAthleteId)
				|| (c.ChallengeeAthleteId == item.ChallengerAthleteId && c.ChallengerAthleteId == item.ChallengeeAthleteId))
				&& c.LeagueId == item.LeagueId).OrderByDescending(c => c.DateCompleted);

			var league = _context.Leagues.SingleOrDefault(l => l.Id == item.LeagueId);
			var lastChallenge = history.FirstOrDefault();
			
			if(lastChallenge != null && lastChallenge.DateCompleted != null
				&& lastChallenge.ChallengerAthleteId == item.ChallengerAthleteId //is it the same athlete challenging again
				&& lastChallenge.GetChallengerWinningGames().Count() < lastChallenge.GetChallengeeWinningGames().Count() //did the challenger lose the previous match
				&& DateTime.UtcNow.Subtract(lastChallenge.DateCompleted.Value.UtcDateTime).TotalHours < league.MinHoursBetweenChallenge) //has enough time passed
			{
				throw "You must wait at least {0} hours before challenging again".Fmt(league.MinHoursBetweenChallenge).ToException(Request);
			}

			Challenge current = await InsertAsync(item.ToChallenge());
            var result = CreatedAtRoute("Tables", new { id = current.Id }, current.ToChallengeDto());

			var message = "{0}: YOU HAVE BEEN CHALLENGED by {1}!".Fmt(league.Name, challenger.Alias);
			var payload = new NotificationPayload
			{
				Action = PushActions.ChallengePosted,
				Payload = { { "challengeId", current.Id } }
			};

			await _notificationController.NotifyByTag(message, current.ChallengeeAthleteId, payload);

			return result;
        }

		[HttpGet]
		[Route("api/revokeChallenge")]
		public async Task RevokeChallenge(string id)
        {
			var challenge = Lookup(id).Queryable.FirstOrDefault();
			var challenger = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengerAthleteId);
			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);

			await DeleteAsync(id);

			var message = "Your challenge with {0} has been revoked.".Fmt(challenger.Name);
			var payload = new NotificationPayload
			{
				Action = PushActions.ChallengeRevoked,
				Payload = { { "challengeId", id } }
			};

			await _notificationController.NotifyByTag(message, challenge.ChallengeeAthleteId, payload);
        }

		[HttpGet]
		[Route("api/declineChallenge")]
		public async Task DeclineChallenge(string id)
		{
			var challenge = Lookup(id).Queryable.FirstOrDefault();
			var challenger = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengerAthleteId);
			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);

			var message = "Your challenge with {0} has been declined.".Fmt(challengee.Name);
			var payload = new NotificationPayload
			{
				Action = PushActions.ChallengeDeclined,
				Payload = { { "challengeId", id } }
			};

			DeleteAsync(id);
			await _notificationController.NotifyByTag(message, challenge.ChallengerAthleteId, payload);
		}

		[Route("api/acceptChallenge")]
		async public Task<ChallengeDto> AcceptChallenge(string id)
		{
			var challenge = _context.Challenges.SingleOrDefault(c => c.Id == id);
			challenge.DateAccepted = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			var league = _context.Leagues.SingleOrDefault(l => l.Id == challenge.LeagueId);
			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);
			var message = "{0}: Your challenge with {1} has been accepted! MATCH ON!!".Fmt(league.Name, challengee.Name);
			var payload = new NotificationPayload
			{
				Action = PushActions.ChallengeAccepted,
				Payload = { { "challengeId", id } }
			};

			await _notificationController.NotifyByTag(message, challenge.ChallengerAthleteId, payload);
			return challenge.ToChallengeDto();
		}

		[Route("api/postMatchResults")]
		async public Task<ChallengeDto> PostMatchResults(List<GameResultDto> results)
		{
			if(results.Count < 1)
				throw "No game scores were submitted.".ToException(Request);

			var challengeId = results.First().ChallengeId;
			var challenge = _context.Challenges.SingleOrDefault(c => c.Id == challengeId);

			if(challenge.DateCompleted != null)
				throw "Scores for this challenge have already been submitted.".ToException(Request);

			var league = _context.Leagues.SingleOrDefault(l => l.Id == challenge.LeagueId);
			var tempChallenge = new Challenge();
			tempChallenge.League = league;
			tempChallenge.MatchResult = results.Select(g => g.ToGameResult()).ToList();

			var errorMessage = tempChallenge.ValidateMatchResults();

			if(errorMessage != null)
			{
				throw errorMessage.ToException(Request);
			}

			tempChallenge = null;
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

				var winningAthleteId = challengee.Id;
				var losingAthleteId = challenger.Id;

				if(challengerWins.Length > challengeeWins.Length)
				{
					challenger = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengerAthleteId);
					challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);

					var oldRank = challengerMembership.CurrentRank;
					challengerMembership.CurrentRank = challengeeMembership.CurrentRank;
					challengeeMembership.CurrentRank = oldRank;
					winningRank = challengerMembership.CurrentRank;
					winningAthleteId = challenger.Id;
					losingAthleteId = challengee.Id;

					_context.SaveChanges();
				}

				if(challengeeWins.Length > challengerWins.Length)
				{
					challenger = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);
					challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengerAthleteId);
				}

				var maintain = challenge.ChallengerAthlete.Id == challenger.Id ? "bequeath" : "retain";
				var newRank = winningRank + 1;
				var message = "{0} victors over {1} to {2} the righteous rank of {3} place in {4}".Fmt(challenger.Alias, challengee.Alias, maintain, newRank.ToOrdinal(), league.Name);
				var payload = new NotificationPayload
				{
					Action = PushActions.ChallengeCompleted,
					Payload = { { "challengeId", challengeId },
						{ "leagueId", league.Id },
						{"winningAthleteId", winningAthleteId },
						{"losingAthleteId", losingAthleteId} }
				};

				await _notificationController.NotifyByTag(message, challenge.LeagueId, payload);
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
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			return dto;
		}
	}
}