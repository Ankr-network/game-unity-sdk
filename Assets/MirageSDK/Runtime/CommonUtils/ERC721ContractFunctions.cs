using System.Numerics;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Data.ContractMessages.ERC721;
using Cysharp.Threading.Tasks;

namespace MirageSDK.CommonUtils
{
	/// <summary>
	///     Class <c>ContractFunctions</c> Contains functions usable by all ERC721 standard contracts.
	/// </summary>
	public static class ERC721ContractFunctions
	{
		private static UniTask<string> CallContractMethod(this IContract contract, string methodName,
			object[] arguments)
		{
			return contract.CallMethod(methodName, arguments, "30000");
		}

		#region ERC721 Standard

		#region Miningless

		/// <summary>
		///     Returns the number of tokens in
		///     <paramref name="owner" />'s account.
		/// </summary>
		public static UniTask<BigInteger> BalanceOf(this IContract contract, string owner)
		{
			var balanceOfMessage = new BalanceOfMessage
			{
				Owner = owner
			};

			return contract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);
		}

		/// <summary>
		///     Returns the owner of the
		///     <paramref name="tokenId" /> token.<br />
		///     Requirement:<br />
		///     -"<paramref name="tokenId" />" must exist
		/// </summary>
		public static UniTask<string> OwnerOf(this IContract contract, BigInteger tokenId)
		{
			var ownerOfMessage = new OwnerOfMessage
			{
				TokenId = tokenId
			};

			return contract.GetData<OwnerOfMessage, string>(ownerOfMessage);
		}

		//IERC721 Metadata
		/// <summary>Returns the contract "<paramref name="contract" />"'s collection name.</summary>
		public static UniTask<string> Name(this IContract contract)
		{
			var nameMessage = new NameMessage();

			return contract.GetData<NameMessage, string>(nameMessage);
		}

		/// <summary>Returns the contract "<paramref name="contract" />"'s collection symbol.</summary>
		public static async UniTask<string> Symbol(this IContract contract, string tokenID)
		{
			var symbolMessage = new SymbolMessage();
			var tokenSymbol = await contract.GetData<SymbolMessage, string>(symbolMessage);

			return tokenSymbol;
		}

		/// <summary>Returns the Uniform Resource Identifier (URI) for "<paramref name="tokenId" />" token.</summary>
		public static UniTask<string> TokenURI(this IContract contract, BigInteger tokenId)
		{
			var tokenURIMessage = new TokenURIMessage
			{
				TokenId = tokenId
			};

			return contract.GetData<TokenURIMessage, string>(tokenURIMessage);
		}

		// ERC-721 Non-Fungible Token Standard, optional enumeration extension
		// See https://eips.ethereum.org/EIPS/eip-721
		/// <summary>Returns the total amount of tokens stored by the contract.</summary>
		public static UniTask<BigInteger> TotalSupply(this IContract contract)
		{
			var totalSupplyMessage = new TotalSupplyMessage();

			return contract.GetData<TotalSupplyMessage, BigInteger>(totalSupplyMessage);
		}

		/// <summary>
		///     Returns a token ID owned by "<paramref name="owner" />"  at a given "<paramref name="index" />" of its token list.
		///     <br />
		///     Use along with <see cref="BalanceOf" /> to enumerate all of "<paramref name="owner" />"'s tokens.
		/// </summary>
		public static UniTask<BigInteger> TokenOfOwnerByIndex(this IContract contract, string owner, BigInteger index)
		{
			var tokenOfOwnerByIndexMessage = new TokenOfOwnerByIndexMessage
			{
				Owner = owner,
				Index = index
			};

			return contract.GetData<TokenOfOwnerByIndexMessage, BigInteger>(tokenOfOwnerByIndexMessage);
		}

		/// <summary>
		///     Returns a token ID at a given "<paramref name="index" />" of all the tokens stored by the contract.<br />
		///     Use along with <see cref="TotalSupply" /> to enumerate all tokens.
		/// </summary>
		public static UniTask<BigInteger> TokenByIndex(this IContract contract, BigInteger index)
		{
			var tokenByIndexMessage = new TokenByIndexMessage
			{
				Index = index
			};

			return contract.GetData<TokenByIndexMessage, BigInteger>(tokenByIndexMessage);
		}

		/// <summary>
		///     Returns if the `operator` is allowed to manage all of the assets of `owner`.<br />
		///     See <see cref="SetApprovalForAll" />
		/// </summary>
		public static UniTask<bool> IsApprovedForAll(this IContract contract, string owner, string theOperator)
		{
			var isApprovedForAllMessage = new IsApprovedForAllMessage
			{
				Owner = owner,
				Operator = theOperator
			};

			return contract.GetData<IsApprovedForAllMessage, bool>(isApprovedForAllMessage);
		}

		/// <summary>
		///     Returns the account approved for <paramref name="tokenId" /> token.<br />
		///     Requirement:<br />
		///     -"<paramref name="tokenId" />" must exist
		/// </summary>
		public static UniTask<bool> GetApproved(this IContract contract, BigInteger tokenId)
		{
			var getApprovedMessage = new GetApprovedMessage
			{
				TokenID = tokenId
			};

			return contract.GetData<GetApprovedMessage, bool>(getApprovedMessage);
		}

		#endregion

		#region Mining Required

		/// <summary>
		///     Safely transfers "<paramref name="tokenId" />" token from "<paramref name="from" />"  to "<paramref name="to" />",
		///     checking first that contract recipients are aware of the ERC721 protocol to prevent tokens from being forever
		///     locked.<br />
		///     Requirements:<br />
		///     - "<paramref name="from" />" cannot be the zero address.<br />
		///     - "<paramref name="to" />" cannot be the zero address.<br />
		///     - "<paramref name="tokenId" />" token must be owned by "<paramref name="from" />".<br />
		///     - If the caller is not "<paramref name="from" />", it must be approved to move this token by either
		///     <see cref="Approve" /> or <see cref="SetApprovalForAll" />.<br />
		///     - If "<paramref name="to" />" refers to a smart contract, it must implement {IERC721Receiver-onERC721Received},
		///     which is called upon a safe transfer.
		/// </summary>
		/// <returns>The transaction hash in a string format</returns>
		public static UniTask<string> SafeTransferFrom(this IContract contract, string from, string to,
			BigInteger tokenId)
		{
			const string safeTransferFromMethodName = "safeTransferFrom";

			return CallContractMethod(contract, safeTransferFromMethodName, new object[]
			{
				from,
				to,
				tokenId
			});
		}

		public static UniTask<string> SafeTransferFrom(this IContract contract, string from, string to,
			BigInteger tokenId, byte data)
		{
			const string safeTransferFromMethodName = "safeTransferFrom";

			return CallContractMethod(contract, safeTransferFromMethodName, new object[]
			{
				from,
				to,
				tokenId,
				data
			});
		}

		/// <summary>
		///     Transfers "<paramref name="tokenId" />" token from "<paramref name="from" />"  to "<paramref name="to" />"<br />
		///     WARNING: Usage of this method is discouraged, use <see cref="SafeTransferFrom" /> whenever possible.<br />
		///     Requirements:<br />
		///     - "<paramref name="from" />" cannot be the zero address.<br />
		///     - "<paramref name="to" />" cannot be the zero address.<br />
		///     - "<paramref name="tokenId" />" token must be owned by "<paramref name="from" />".<br />
		///     - If the caller is not "<paramref name="from" />", it must be approved to move this token by either
		///     <see cref="Approve" /> or <see cref="SetApprovalForAll" />.
		/// </summary>
		/// <returns>The transaction hash in a string format</returns>
		public static UniTask<string> TransferFrom(this IContract contract, string from, string to, BigInteger tokenId)
		{
			const string transferFromMethodName = "transferFrom";

			return CallContractMethod(contract, transferFromMethodName, new object[]
			{
				from,
				to,
				tokenId
			});
		}

		/// <summary>
		///     Gives permission to "<paramref name="to" />" to transfer "<paramref name="tokenId" />" token to another account.
		///     The approval is cleared when the token is transferred.<br />
		///     Only a single account can be approved at a time, so approving the zero address clears previous approvals.<br />
		///     Requirements:<br />
		///     - The caller must own the token "<paramref name="tokenId" />" or be an approved operator.<br />
		///     - "<paramref name="tokenId" />" must exist.<br />
		/// </summary>
		/// <returns>The transaction hash in a string format</returns>
		public static UniTask<string> Approve(this IContract contract, string to, string tokenId)
		{
			const string approveMethodName = "approve";

			return CallContractMethod(contract, approveMethodName, new object[]
			{
				to,
				tokenId
			});
		}

		/// <summary>
		///     Approve or remove "<paramref name="callOperator" />" as an operator for the caller.
		///     Operators can call <see cref="TransferFrom" /> or <see cref="SafeTransferFrom" /> for any token owned by the
		///     caller.<br />
		///     Requirements:<br />
		///     - The "<paramref name="callOperator" />"  cannot be the caller.
		/// </summary>
		/// <returns>The transaction hash in a string format</returns>
		public static UniTask<string> SetApprovalForAll(this IContract contract, string callOperator, bool approved)
		{
			const string setApprovalForAllMethodName = "setApprovalForAll";

			return CallContractMethod(contract, setApprovalForAllMethodName, new object[]
			{
				callOperator,
				approved
			});
		}

		#endregion

		#endregion
	}
}