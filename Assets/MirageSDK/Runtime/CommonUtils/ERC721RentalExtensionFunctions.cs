using System.Numerics;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data.ContractMessages.ERC721.RentableExtension;
using Cysharp.Threading.Tasks;

namespace MirageSDK.CommonUtils
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
		public static UniTask<string> PrincipalOwner(this IContract contract, BigInteger tokenId)
		{
			var principalOwnerMessage = new PrincipalOwnerMessage
			{
				TokenId = tokenId
			};

			return contract.GetData<PrincipalOwnerMessage, string>(principalOwnerMessage);
		}

		/// <summary>
		///     Get whether or not the token "<paramref name="tokenId" />" is rented.
		/// </summary>
		public static UniTask<bool> IsRented(this IContract contract, BigInteger tokenId)
		{
			var isRentedMessage = new IsRentedMessage
			{
				TokenId = tokenId
			};

			return contract.GetData<IsRentedMessage, bool>(isRentedMessage);
		}

		#endregion

		#region Mining Required

		/// <summary>
		///     Rent a token to another address "<paramref name="renter" />" as an operator for the caller.
		/// </summary>
		public static UniTask<string> RentOut(this IContract contract, string renter, string tokenId,
			BigInteger expiresAt)
		{
			const string rentOutMethodName = "rentOut";

			return contract.CallMethod(rentOutMethodName, new object[]
			{
				renter,
				tokenId,
				expiresAt
			});
		}

		/// <summary>
		///     Renter can run this anytime but owner can run after the expired time.
		/// </summary>
		public static UniTask<string> FinishRenting(this IContract contract, string tokenId)
		{
			const string finishRentingMethodName = "finishRenting";

			return contract.CallMethod(finishRentingMethodName, new object[]
			{
				tokenId
			});
		}

		#endregion
	}
}