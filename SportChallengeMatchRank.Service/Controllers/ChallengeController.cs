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

		IQueryable<ChallengeDto> ConvertChallengeToDto(IQueryable<Challenge> queryable)
		{
			return queryable.Select(c => new ChallengeDto
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

        // GET tables/Challenge
		public IQueryable<ChallengeDto> GetAllChallenges()
		{
			return ConvertChallengeToDto(Query());
		}
		        // GET tables/Challenge/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<ChallengeDto> GetChallenge(string id)
        {
			return SingleResult<ChallengeDto>.Create(ConvertChallengeToDto(Lookup(id).Queryable));
		}

		[Route("api/getChallengesForAthlete")]
		public IQueryable<ChallengeDto> GetChallengesForAthlete(string athleteId)
		{
			return ConvertChallengeToDto(Query().Where(c => c.ChallengeeAthleteId == athleteId
				|| c.ChallengerAthleteId == athleteId));
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

			if(challenger == null || challengee == null)
				throw "The opponent in this challenge no longer belongs to this league".ToException(Request);

			var challengerMembership = _context.Memberships.SingleOrDefault(m => m.AbandonDate == null && m.AthleteId == challenger.Id && m.LeagueId == item.LeagueId);
			var challengeeMembership = _context.Memberships.SingleOrDefault(m => m.AbandonDate == null && m.AthleteId == challengee.Id && m.LeagueId == item.LeagueId);

			if(challengerMembership == null || challengeeMembership == null)
				throw "The opponent in this challenge no longer belongs to this league".ToException(Request);

			//Check to see if there are any ongoing challenges between either athlete
			var challengeeOngoing = _context.Challenges.Where(c => (c.ChallengeeAthleteId == item.ChallengeeAthleteId || c.ChallengerAthleteId == item.ChallengeeAthleteId)
				&& c.LeagueId == item.LeagueId && c.DateCompleted == null);

			if(challengeeOngoing.Count() > 0)
				throw "{0} already has an existing challenge underway.".Fmt(challengee.Alias).ToException(Request);

			var challengerOngoing = _context.Challenges.Where(c => (c.ChallengerAthleteId == item.ChallengerAthleteId || c.ChallengeeAthleteId == item.ChallengerAthleteId)
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

			var message = "{0}: You have been challenged to a duel by {1}!".Fmt(league.Name, challenger.Alias);
			var payload = new NotificationPayload
			{
				Action = PushActions.ChallengePosted,
				Payload = { { "challengeId", current.Id }, { "leagueId", current.LeagueId } }
			};

			//Not awaiting so the user's result is not delayed
			_notificationController.NotifyByTag(message, current.ChallengeeAthleteId, payload);
			return result;
        }

		[HttpGet]
		[Route("api/revokeChallenge")]
		public async Task RevokeChallenge(string id)
        {
			var challenge = Lookup(id).Queryable.FirstOrDefault();

			if(challenge == null)
				return;

			var challenger = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengerAthleteId);
			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);

			var message = "Your challenge with {0} has been revoked.".Fmt(challenger.Name);
			var payload = new NotificationPayload
			{
				Action = PushActions.ChallengeRevoked,
				Payload = { { "challengeId", id }, { "leagueId", challenge.LeagueId } }
			};

			await _notificationController.NotifyByTag(message, challenge.ChallengeeAthleteId, payload);
			await DeleteAsync(id);
		}

		[HttpGet]
		[Route("api/declineChallenge")]
		public async Task DeclineChallenge(string id)
		{
			var challenge = Lookup(id).Queryable.FirstOrDefault();

			if(challenge == null)
				return;

			var challenger = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengerAthleteId);
			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);

			var message = "Your challenge with {0} has been declined.".Fmt(challengee.Name);
			var payload = new NotificationPayload
			{
				Action = PushActions.ChallengeDeclined,
				Payload = { { "challengeId", id }, { "leagueId", challenge.LeagueId } }
			};

			await _notificationController.NotifyByTag(message, challenge.ChallengerAthleteId, payload);
			await DeleteAsync(id);
		}

		[Route("api/acceptChallenge")]
		async public Task<ChallengeDto> AcceptChallenge(string id)
		{
			var challenge = _context.Challenges.SingleOrDefault(c => c.Id == id);

			if(challenge == null)
				throw "This challenge no longer exists".ToException(Request);

			challenge.DateAccepted = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			var league = _context.Leagues.SingleOrDefault(l => l.Id == challenge.LeagueId);
			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);
			var message = "{0}: Your challenge with {1} has been accepted!".Fmt(league.Name, challengee.Name);
			var payload = new NotificationPayload
			{
				Action = PushActions.ChallengeAccepted,
				Payload = { { "challengeId", id }, { "leagueId", challenge.LeagueId } }
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

			if(challenge == null)
				throw "This challenge no longer exists".ToException(Request);

			if(challenge.DateCompleted != null)
				throw "Scores for this challenge have already been submitted.".ToException(Request);

			var league = _context.Leagues.SingleOrDefault(l => l.Id == challenge.LeagueId);

			if(league == null)
				throw "This league no longer exists".ToException(Request);

			var challenger = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengerAthleteId);
			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == challenge.ChallengeeAthleteId);

			if(challenger == null || challengee == null)
				throw "The opponent in this challenge no longer belongs to this league".ToException(Request);

			var challengerMembership = _context.Memberships.SingleOrDefault(m => m.AthleteId == challenger.Id && m.AbandonDate== null && m.LeagueId == challenge.LeagueId);
			var challengeeMembership = _context.Memberships.SingleOrDefault(m => m.AthleteId == challengee.Id && m.AbandonDate == null && m.LeagueId == challenge.LeagueId);

			if(challengerMembership == null || challengeeMembership == null)
				throw "The opponent in this challenge no longer belongs to this league".ToException(Request);

			var tempChallenge = new Challenge();
			tempChallenge.League = league;
			tempChallenge.MatchResult = results.Select(g => g.ToGameResult()).ToList();

			var errorMessage = tempChallenge.ValidateMatchResults();

			if(errorMessage != null)
				throw errorMessage.ToException(Request);

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

				_notificationController.NotifyByTag(message, challenge.LeagueId, payload);
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