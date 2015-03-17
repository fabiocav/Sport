using System;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared.Mobile
{
	public interface IAuthenticationHandler
	{
		Task AuthenticateUser();
	}
}