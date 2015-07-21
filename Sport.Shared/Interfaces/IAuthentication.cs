using System;
using System.Threading.Tasks;

namespace Sport.Shared
{
	public interface IAuthentication
	{
		Task<Tuple<string, string>> AuthenticateUser();
	}
}

