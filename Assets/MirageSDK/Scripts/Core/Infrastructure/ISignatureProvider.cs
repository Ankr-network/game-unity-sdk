using System.Threading.Tasks;

namespace MirageSDK.Core.Infrastructure
{
	public interface ISignatureProvider
	{
		Task<string> Sign(string messageToSign);
		string CheckSignature(string messageToCheck, string signature);
	}
}