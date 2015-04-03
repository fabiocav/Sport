using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportChallengeMatchRank.Service.Models;

namespace SportChallengeMatchRank.Service.Controllers
{
	//[AuthorizeLevel(AuthorizationLevel.User)]
	public class GameResultController : TableController<GameResult>
	{
		AppDataContext _context = new AppDataContext();

		protected override void Initialize(HttpControllerContext controllerContext)
		{
			base.Initialize(controllerContext);
			DomainManager = new EntityDomainManager<GameResult>(_context, Request, Services);
		}

		// GET tables/GameResult
		public IQueryable<GameResultDto> GetAllGameResults()
		{
			return Query().Select(dto => new GameResultDto
			{
				Id = dto.Id,
				DateCreated = dto.CreatedAt,
				ChallengeId = dto.ChallengeId,
				ChallengeeScore = dto.ChallengeeScore,
				ChallengerScore = dto.ChallengerScore,
				Index = dto.Index
			});
		}

		// GET tables/GameResult/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public SingleResult<GameResultDto> GetGameResult(string id)
		{
			return SingleResult<GameResultDto>.Create(Lookup(id).Queryable.Select(dto => new GameResultDto
			{
				Id = dto.Id,
				ChallengeId = dto.ChallengeId,
				ChallengeeScore = dto.ChallengeeScore,
				ChallengerScore = dto.ChallengerScore,
				Index = dto.Index
			}));
		}

		// PATCH tables/GameResult/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public Task<GameResult> PatchGameResult(string id, Delta<GameResult> patch)
		{
			return UpdateAsync(id, patch);
		}

		// POST tables/GameResult
		public async Task<IHttpActionResult> PostGameResult(GameResultDto item)
		{
			GameResult current = await InsertAsync(item.ToGameResult());
			var result = CreatedAtRoute("Tables", new { id = current.Id }, current);
			return result;
		}

		// DELETE tables/GameResult/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public Task DeleteGameResult(string id)
		{
			var task = DeleteAsync(id);
			return task;
		}
	}
}