using System.Threading.Tasks;

namespace MirageSDK.Core.Infrastructure
{
	public interface IMirageSDK
	{
		IContract GetContract(string address, string abi);
		Task<string> Sign(string message);
		string CheckSignature(string message, string signature);
	}
}