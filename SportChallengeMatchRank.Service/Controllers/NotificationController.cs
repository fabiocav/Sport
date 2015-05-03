using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using SportChallengeMatchRank.Service.Models;
using System.Net;
using System.Net.Http;
using Microsoft.ServiceBus.Notifications;
using System.Collections.Generic;
using Microsoft.ServiceBus.Messaging;
using SportChallengeMatchRank.Shared;

namespace SportChallengeMatchRank.Service.Controllers
{
	public class NotificationController : ApiController
	{
		AppDataContext _context = new AppDataContext();
		NotificationHubClient _hub;

		protected override void Initialize(HttpControllerContext controllerContext)
		{
			_hub = NotificationHubClient.CreateClientFromConnectionString(Constants.HubConnectionString, Constants.HubName);
			base.Initialize(controllerContext);
		}

		#region Notification

		[HttpPost]
		[Route("api/notifyByTags")]
		public async Task<HttpResponseMessage> Post(string message, List<string> tags)
		{
			var notification = new Dictionary<string, string> { { "message", message } };
			await _hub.SendTemplateNotificationAsync(notification, tags);
			return Request.CreateResponse(HttpStatusCode.OK);
		}

		#endregion

		#region Registration

		[HttpPut]
		[Route("api/registerWithHub")]
		public async Task<string> RegisterWithHub(DeviceRegistration deviceUpdate)
		{
			string newRegistrationId = null;

			// make sure there are no existing registrations for this push handle (used for iOS and Android)
			if(deviceUpdate.Handle != null)
			{
				var registrations = await _hub.GetRegistrationsByChannelAsync(deviceUpdate.Handle, 100);

				foreach(var reg in registrations)
				{
					if(newRegistrationId == null)
					{
						newRegistrationId = reg.RegistrationId;
					}
					else
					{
						await _hub.DeleteRegistrationAsync(reg);
					}
				}
			}

			if(newRegistrationId == null)
				newRegistrationId = await _hub.CreateRegistrationIdAsync();

			RegistrationDescription registration = null;

			switch(deviceUpdate.Platform)
			{
				case "mpns":
					var toastTemplate = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
						"<wp:Notification xmlns:wp=\"WPNotification\">" +
						   "<wp:Toast>" +
								"<wp:Text1>$(message)</wp:Text1>" +
						   "</wp:Toast> " +
						"</wp:Notification>";
					registration = new MpnsTemplateRegistrationDescription(deviceUpdate.Handle, toastTemplate);
					break;
				case "wns":
					toastTemplate = @"<toast><visual><binding template=""ToastText01""><text id=""1"">$(message)</text></binding></visual></toast>";
					registration = new WindowsTemplateRegistrationDescription(deviceUpdate.Handle, toastTemplate);
					break;
				case "apns":
					var alertTemplate = "{\"aps\":{\"alert\":\"$(message)\"}}";
					registration = new AppleTemplateRegistrationDescription(deviceUpdate.Handle, alertTemplate);
					break;
				case "gcm":
					var messageTemplate = "{\"data\":{\"msg\":\"$(message)\"}}";
					registration = new GcmTemplateRegistrationDescription(deviceUpdate.Handle, messageTemplate);
					break;
				default:
					throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			registration.RegistrationId = newRegistrationId;
			registration.Tags = new HashSet<string>(deviceUpdate.Tags);
			try
			{
				await _hub.CreateOrUpdateRegistrationAsync(registration);
			}
			catch(MessagingException e)
			{
				ReturnGoneIfHubResponseIsGone(e);
			}

			return newRegistrationId;
		}

		public async Task<HttpResponseMessage> Delete(string id)
		{
			await _hub.DeleteRegistrationAsync(id);
			return Request.CreateResponse(HttpStatusCode.OK);
		}

		private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
		{
			var webex = e.InnerException as WebException;
			if(webex == null || webex.Status != WebExceptionStatus.ProtocolError)
				return;
			var response = (HttpWebResponse)webex.Response;
			if(response.StatusCode == HttpStatusCode.Gone)
				throw new HttpRequestException(HttpStatusCode.Gone.ToString());
		}

		#endregion
	}
}