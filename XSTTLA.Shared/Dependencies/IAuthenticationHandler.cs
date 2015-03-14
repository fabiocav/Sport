using System;
using System.Threading.Tasks;

namespace XSTTLA.Shared
{
	public interface IAuthenticationHandler
	{
		Task AuthenticateUser();
	}
}