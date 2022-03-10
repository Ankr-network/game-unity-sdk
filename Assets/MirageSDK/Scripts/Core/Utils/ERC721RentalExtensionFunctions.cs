using System.Numerics;
using Cysharp.Threading.Tasks;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Examples.ContractMessages.ERC721.RentableExtension;

namespace MirageSDK.Core.Utils
{
	/// <summary>
	///     Class <c>ContractFunctions</c> Contains functions usable by ERC721 contracts implementing the Rental Extension.
	/// </summary>
	public static class ERC721RentalExtensionFunctions
	{
		#region Miningless

		/// <summary>
		///     Get the actual owner of the rented token "<paramref name="tokenId" />".
		/// </summary>
		public static UniTask<string> PrincipalOwner(string tokenId, IContract contract)
		{
			var principalOwnerMessage = new PrincipalOwnerMessage
			{
				TokenId = tokenId
			};

			return contract.GetData<PrincipalOwnerMessage, string>(principalOwnerMessage).AsUniTask();
		}

		/// <summary>
		///     Get whether or not the token "<paramref name="tokenId" />" is rented.
		/// </summary>
		public static UniTask<bool> IsRented(string tokenId, IContract contract)
		{
			var isRentedMessage = new IsRentedMessage
			{
				TokenId = tokenId
			};

			return contract.GetData<IsRentedMessage, bool>(isRentedMessage).AsUniTask();
		}

		#endregion

		#region Mining Required

		/// <summary>
		///     Rent a token to another address "<paramref name="renter" />" as an operator for the caller.
		/// </summary>
		public static UniTask<string> RentOut(string renter, string tokenId, BigInteger expiresAt, IContract contract)
		{
			const string rentOutMethodName = "rentOut";

			return contract.CallMethod(rentOutMethodName, new object[]
			{
				renter,
				tokenId,
				expiresAt
			}).AsUniTask();
		}

		/// <summary>
		///     Renter can run this anytime but owner can run after the expired time.
		/// </summary>
		public static UniTask<string> FinishRenting(string tokenId, IContract contract)
		{
			const string finishRentingMethodName = "finishRenting";

			return contract.CallMethod(finishRentingMethodName, new object[]
			{
				tokenId
			}).AsUniTask();
		}

		#endregion
	}
}