# Update NFT

This is an example for one of the use cases for this sdk : <br/>Checking if someone owns an NFTs. This use case is usefull when you want to know if someone has specific NFTs to then let him use them ingame. 

### How To
To start this Use Case we will first need to initialize the SDKWrapper and store the contracts that we will use here inside some private variables. For this example we will reuse the GameCharacterContract used in the WearableNFTExample Use Case.

```c#
            var mirageSDKWrapper = MirageSDKWrapper.GetInitializedInstance(WearableNFTContractInformation.ProviderURL);
			_gameCharacterContract = mirageSDKWrapper.GetContract(WearableNFTContractInformation.GameCharacterContractAddress, WearableNFTContractInformation.GameCharacterABI);

```

If you want to know if the player has certain NFTs you can call the balanceOf function, which will return the number of these Tokens in the address.

```solidity
    /**
     * @dev Returns the number of tokens in ``owner``'s account.
     */
    function balanceOf(address owner) external view returns (uint256 balance);

```

If you need to get the ID of a specific NFT you can use the tokenOfOwnerByIndex

```solidity
    /**
     * @dev Returns a token ID owned by `owner` at a given `index` of its token list.
     * Use along with {balanceOf} to enumerate all of ``owner``'s tokens.
     */
    function tokenOfOwnerByIndex(address owner, uint256 index) external view returns (uint256 tokenId);

```

Once you have checked that the authenticated wallet has an NFT you can then give access to it for the user. You can for exemple let him use Hero X if he has the Hero X NFT.