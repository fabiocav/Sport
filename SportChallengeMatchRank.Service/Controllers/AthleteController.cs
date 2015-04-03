﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using SportChallengeMatchRank;
using SportChallengeMatchRank.Service.Models;
using System;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Net.Http;

namespace SportChallengeMatchRank.Service.Controllers
{
	//[AuthorizeLevel(AuthorizationLevel.User)]
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
			return Query().Select(dto => new AthleteDto
			{
				Name = dto.Name,
				Id = dto.Id,
				Email = dto.Email,
				DateCreated = dto.CreatedAt,
				IsAdmin = dto.IsAdmin,
				DeviceToken = dto.DeviceToken,
				DevicePlatform = dto.DevicePlatform,
				AuthenticationId = dto.AuthenticationId,
				MembershipIds = dto.Memberships.Select(la => la.Id).ToList()
			});
		}

		// GET tables/Athlete/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public SingleResult<AthleteDto> GetAthlete(string id)
		{
			return SingleResult<AthleteDto>.Create(Lookup(id).Queryable.Select(dto => new AthleteDto
			{
				Name = dto.Name,
				Id = dto.Id,
				DateCreated = dto.CreatedAt,
				Email = dto.Email,
				IsAdmin = dto.IsAdmin,
				DeviceToken = dto.DeviceToken,
				DevicePlatform = dto.DevicePlatform,
				AuthenticationId = dto.AuthenticationId,
				MembershipIds = dto.Memberships.Select(la => la.Id).ToList()
			}));
		}

		// PATCH tables/Athlete/48D68C86-6EA6-4C25-AA33-223FC9A27959
		async public Task<Athlete> PatchAthlete(string id, Delta<Athlete> patch)
		{
			var athlete = patch.GetEntity();
			return await UpdateAsync(id, patch);
		}

		// POST tables/Athlete
		public async Task<IHttpActionResult> PostAthlete(AthleteDto item)
		{
			var exists = _context.Athletes.Any(l => l.Email.Equals(item.Email, System.StringComparison.InvariantCultureIgnoreCase));

			if(exists)
				return Conflict();

			Athlete athlete = await InsertAsync(item.ToAthlete());
			var message = new ApplePushMessage("Thanks for registering, {0}".Fmt(athlete.Name), TimeSpan.FromHours(1));

			try
			{
				if(athlete.DeviceToken != null)
				{
					var result = await Services.Push.SendAsync(message, athlete.DeviceToken);
					Services.Log.Info(result.State.ToString());
				}
			}
			catch(System.Exception ex)
			{
				Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
			}

			return CreatedAtRoute("Tables", new
			{
				id = athlete.Id
			}, athlete);
		}

		// DELETE tables/Athlete/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public Task DeleteAthlete(string id)
		{
			return DeleteAsync(id);
		}
	}
}