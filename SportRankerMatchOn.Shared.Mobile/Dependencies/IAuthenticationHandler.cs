using System;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared
{
	public interface IAuthenticationHandler
	{
		Task AuthenticateUser();
	}
}