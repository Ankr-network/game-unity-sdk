using System.Numerics;
using Cysharp.Threading.Tasks;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Examples.Scripts.ContractMessages;
using MirageSDK.Examples.Scripts.ContractMessages.GameCharacterContract;

namespace MirageSDK.Core.Utils
{
	/// <summary>
	///     Class <c>ContractFunctions</c> Contains functions usable by all ERC721 standard contracts.
	/// </summary>
	public class ContractFunctions
	{
		#region ERC721 Standard

		#region Miningless

		/// <summary>
		///     Returns the number of tokens in
		///     <paramref name="owner" />'s account.
		/// </summary>
		private async UniTask<BigInteger> BalanceOf(string owner, IContract contract)
		{
			var balanceOfMessage = new BalanceOfMessage
			{
				Owner = owner
			};
			var balance = await contract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);

			return balance;
		}

		/// <summary>
		///     Returns the owner of the
		///     <paramref name="tokenId" /> token.<br />
		///     Requirement:<br />
		///     -"<paramref name="tokenId" />" must exist
		/// </summary>
		private async UniTask<string> OwnerOf(string tokenId, IContract contract)
		{
			var ownerOfMessage = new OwnerOfMessage
			{
				TokenId = tokenId
			};
			var owner = await contract.GetData<OwnerOfMessage, string>(ownerOfMessage);

			return owner;
		}

		//IERC721 Metadata
		/// <summary>Returns the token "<paramref name="tokenId" />"'s collection name.</summary>
		private async UniTask<string> Name(IContract contract)
		{
			var nameMessage = new NameMessage();
			var tokenName = await contract.GetData<NameMessage, string>(nameMessage);

			return tokenName;
		}

		/// <summary>Returns the token "<paramref name="tokenId" />"'s collection symbol.</summary>
		private async UniTask<string> Symbol(string tokenID, IContract contract)
		{
			var symbolMessage = new SymbolMessage();
			var tokenSymbol = await contract.GetData<SymbolMessage, string>(symbolMessage);

			return tokenSymbol;
		}

		/// <summary>Returns the Uniform Resource Identifier (URI) for "<paramref name="tokenId" />" token.</summary>
		private async UniTask<string> TokenURI(string tokenID, IContract contract)
		{
			var tokenURIMessage = new TokenURIMessage
			{
				TokenId = tokenID
			};
			var tokenURI = await contract.GetData<TokenURIMessage, string>(tokenURIMessage);

			return tokenURI;
		}

		// ERC-721 Non-Fungible Token Standard, optional enumeration extension
		// See https://eips.ethereum.org/EIPS/eip-721
		/// <summary>Returns the total amount of tokens stored by the contract.</summary>
		private async UniTask<BigInteger> TotalSupply(IContract contract)
		{
			var totalSupplyMessage = new TotalSupplyMessage();
			var totalSupply = await contract.GetData<TotalSupplyMessage, BigInteger>(totalSupplyMessage);

			return totalSupply;
		}

		/// <summary>
		///     Returns a token ID owned by "<paramref name="owner" />"  at a given "<paramref name="index" />" of its token list.
		///     Use along with <see cref="BalanceOf" /> to enumerate all of "<paramref name="owner" />"'s tokens.
		/// </summary>
		private async UniTask<BigInteger> TokenOfOwnerByIndex(string owner, BigInteger index, IContract contract)
		{
			var tokenOfOwnerByIndexMessage = new TokenOfOwnerByIndexMessage
			{
				Owner = owner,
				Index = index
			};
			var tokenId = await contract.GetData<TokenOfOwnerByIndexMessage, BigInteger>(tokenOfOwnerByIndexMessage);

			return tokenId;
		}

		/// <summary>
		///     Returns a token ID at a given "<paramref name="index" />" of all the tokens stored by the contract.
		///     Use along with <see cref="TotalSupply" /> to enumerate all tokens.
		/// </summary>
		private async UniTask<BigInteger> TokenByIndex(BigInteger index, IContract contract)
		{
			var tokenByIndexMessage = new TokenByIndexMessage
			{
				Index = index
			};
			var tokenId = await contract.GetData<TokenByIndexMessage, BigInteger>(tokenByIndexMessage);

			return tokenId;
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
		private async UniTask<string> SafeTransferFrom(string from, string to, BigInteger tokenId, IContract contract)
		{
			const string safeTransferFromMethodName = "safeTransferFrom";
			var transactionHash = await contract.CallMethod(safeTransferFromMethodName, new object[]
			{
				from,
				to,
				tokenId
			});

			return transactionHash;
		}

		private async UniTask<string> SafeTransferFrom(string from, string to, BigInteger tokenId, byte data,
			IContract contract)
		{
			const string safeTransferFromMethodName = "safeTransferFrom";
			var transactionHash = await contract.CallMethod(safeTransferFromMethodName, new object[]
			{
				from,
				to,
				tokenId,
				data
			});

			return transactionHash;
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
		private async UniTask<string> TransferFrom(string from, string to, BigInteger tokenId, IContract contract)
		{
			const string transferFromMethodName = "transferFrom";
			var transactionHash = await contract.CallMethod(transferFromMethodName, new object[]
			{
				from,
				to,
				tokenId
			});

			return transactionHash;
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
		private async UniTask<string> Approve(string to, string tokenId, IContract contract)
		{
			const string approveMethodName = "approve";
			var transactionHash = await contract.CallMethod(approveMethodName, new object[]
			{
				to,
				tokenId
			});

			return transactionHash;
		}

		/// <summary>
		///     Returns the account approved for "<paramref name="tokenId" />" token.
		///     Requirements:<br />
		///     - `tokenId` must exist.
		/// </summary>
		/// <returns>The Operator's address in a string format</returns>
		private async UniTask<string> GetApproved(string tokenId, IContract contract)
		{
			const string getApprovedMethodName = "getApproved";
			var theOperator = await contract.CallMethod(getApprovedMethodName, new object[]
			{
				tokenId
			});

			return theOperator;
		}

		/// <summary>
		///     Approve or remove "<paramref name="callOperator" />" as an operator for the caller.
		///     Operators can call <see cref="TransferFrom" /> or <see cref="SafeTransferFrom" /> for any token owned by the
		///     caller.<br />
		///     Requirements:<br />
		///     - The "<paramref name="callOperator" />"  cannot be the caller.
		/// </summary>
		/// <returns>The transaction hash in a string format</returns>
		private async UniTask<string> SetApprovalForAll(string callOperator, bool approved, IContract contract)
		{
			const string setApprovalForAllMethodName = "setApprovalForAll";
			var transactionHash = await contract.CallMethod(setApprovalForAllMethodName, new object[]
			{
				callOperator,
				approved
			});

			return transactionHash;
		}

		#endregion

		#endregion
	}
}