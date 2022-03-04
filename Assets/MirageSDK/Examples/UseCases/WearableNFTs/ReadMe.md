# Update NFT

This is an example for one of the use cases for this sdk : Wearable/Composable NFTs. This use case should only work if you have the minter rôle but for this demonstration the minter rôle has been removed to allow the testing of all functions. 

### How To
To start this Use Case we will first need to initializethe SDKWrapper and store the contracts that we will use here inside some private variables.

```c#
            var mirageSDKWrapper = MirageSDKWrapper.GetInitializedInstance(WearableNFTContractInformation.ProviderURL);
			_gameCharacterContract = mirageSDKWrapper.GetContract(WearableNFTContractInformation.GameCharacterContractAddress, WearableNFTContractInformation.GameCharacterABI);
			_gameItemContract = mirageSDKWrapper.GetContract(WearableNFTContractInformation.GameItemContractAddress,
				WearableNFTContractInformation.GameItemABI);

```

You can then use the different functions available to interact with the contracts, these are inside the script WearableNFTExample.cs. 

The MintItems and MintCharacter functions are there just to showcase how these would work in a server side environment. Those functions should not work if used by the client because he does not have the minter role and shouldnt have it for security reasons. The minter Role should be given to the server and the client should send a request to mint directly to the server.

The Contract with the minter rôle requirement should look like this :
```solidity
function safeMint(address to) public onlyRole(MINTER_ROLE) {
        uint256 tokenId = _tokenIdCounter.current();
        _tokenIdCounter.increment();
        _safeMint(to, tokenId);
    }

```

Using the ChangeHat Functions you can change the NFT-Hat currently used by the NFT-Character. For this to work you have to link the adress of your hat and you should own the character that you want to equip the hat on as written on the contract. If the character already has a hat equiped the old one will be sent back to your wallet.

```solidity
function changeHat(uint256 characterId, uint256 newHatId) public {
        require(_isHat(newHatId), "Item should be a hat");
        require(ownerOf(characterId) == msg.sender, "Should be owner of character");

        uint256 oldHatId = _hats[characterId];
        if (oldHatId > 0) {
            gameItemContract.safeTransferFrom(address(this), msg.sender, oldHatId, 1, "");
        }
        if (newHatId > 0) {
            gameItemContract.safeTransferFrom(msg.sender, address(this), newHatId, 1, "");
        }
        _hats[characterId] = newHatId;

        emit HatChanged(characterId, oldHatId, newHatId);
    }

```

The GetHat Function will allow you to check which hat is equiped on the character. 
The GetTokenInfo Function will allow you to get informations on the Character NFT you own (if you have multiple ones it will take the 1st). 