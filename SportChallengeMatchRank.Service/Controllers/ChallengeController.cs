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

namespace SportChallengeMatchRank.Service.Controllers
{
	//[AuthorizeLevel(AuthorizationLevel.User)]
	public class ChallengeController : TableController<Challenge>
    {
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
				GameResults = c.GameResults.Select(r => new GameResultDto
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
				GameResults = dto.GameResults.Select(r => new GameResultDto
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
			var exists = _context.Challenges.Any(c => ((c.ChallengeeAthleteId == item.ChallengeeAthleteId && c.ChallengerAthleteId == item.ChallengerAthleteId)
				|| (c.ChallengeeAthleteId == item.ChallengerAthleteId && c.ChallengerAthleteId == item.ChallengeeAthleteId))
				&& c.LeagueId == item.LeagueId && c.DateCompleted == null);

			if(exists)
				return Conflict();

			Challenge current = await InsertAsync(item.ToChallenge());
            var result = CreatedAtRoute("Tables", new { id = current.Id }, current);

			var challenger = _context.Athletes.SingleOrDefault(a => a.Id == current.ChallengerAthleteId);
			var challengee = _context.Athletes.SingleOrDefault(a => a.Id == current.ChallengeeAthleteId);

			try
			{
				var payload = new Dictionary<string, object>{{"challengeId", current.Id}};
				var message = AppDataContext.GetPush(challengee, "YOU HAVE BEEN CHALLENGED by {0}!".Fmt(challenger.Name), payload);
				var pushResult = await Services.Push.SendAsync(message, current.ChallengeeAthleteId);
				Services.Log.Info(pushResult.State.ToString());
			}
			catch(System.Exception ex)
			{
				Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
			}

			return result;
        }

        // DELETE tables/Challenge/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteChallenge(string id)
        {
            var task = DeleteAsync(id);
			//Send push notificaitons
			return task;
        }

		[Route("api/acceptChallenge")]
		async public Task<ChallengeDto> AcceptChallenge(string challengeId)
		{
			var challenge = _context.Challenges.SingleOrDefault(c => c.Id == challengeId);
			challenge.DateAccepted = DateTime.UtcNow;
			await _context.SaveChangesAsync();
			return challenge.ToChallengeDto();
			//Send out push notifcations
		}

		[Route("api/postMatchResults")]
		public Task<ChallengeDto> PostMatchResults(List<GameResultDto> results)
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
			dto.GameResults = new List<GameResultDto>();

			foreach(var result in results)
			{
				result.Id = Guid.NewGuid().ToString();
				_context.GameResults.Add(result.ToGameResult());
				dto.GameResults.Add(result);
			}

			try
			{
				_context.SaveChanges();

				//Rerank the leaderboard

			}
			catch(DbEntityValidationException e)
			{
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
			}

			//Send out push notifcations to entire league

			return dto;
		}
	}
}