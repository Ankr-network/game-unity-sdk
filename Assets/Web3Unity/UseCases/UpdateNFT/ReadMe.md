# Update NFT

This is an example for one of the use cases for this sdk : update nft that belongs to ingame resource by user request.

### Preparing

You need to extend [smart contract](https://github.com/mirage-xyz/mirage-go-demo/blob/main/GameItem.sol) to your purposes and deploy it to network.<br/>
Pay attention that smart contract should be deploy from account that use backend to sign data.

### Steps

To start this use case we need to make instance of `Web3` class and call `Initialize` method after login in metamask

```c#
string provider_url = "<ethereum node url>";
		
Web3 web3 = new Web3(provider_url);
web3.Initialize();
```
---

User should send GET request `hero/{id}` to [backend](https://github.com/mirage-xyz/mirage-go-sdk) where `id` it's index of nft in contract.

```c#
UnityWebRequest request = UnityWebRequest.Get(url+$"hero/{tokenId}");
await request.Send();
```

---

Backend prepared parameters of hero for nft and sign it with [EIP-712](https://eips.ethereum.org/EIPS/eip-712) standard. User will use this signature to prove immutability of the data in contract.

```go
hero := &ItemInfo{
    TokenId:    id,
	ItemType:   1,
	Strength:   10,
	Level:      15,
	ExpireTime: 1642739319576,
}
	
hero.Signature = generateSignature(*hero)
```

---

After getting parameters of nft and signature user should call contract method `updateTokenWithSignedMessage` and pass it all data from the backend as tuple.

```c#
ItemInfo info = await RequestPreparedParams(0);
string receipt = await contract.CallMethod("updateTokenWithSignedMessage", new object[] {info});
```

Contract will verify the signature and expireTime for data and update nft.

```solidity
function updateTokenWithSignedMessage(ItemInfo calldata itemInfo) public {
    address signer = _verifyItemInfo(itemInfo);
    require(hasRole(UPDATER_ROLE, signer), "Signature invalid");
    require(itemInfo.expireTime > block.timestamp, "Voucher expired");
    require(tokenDetails[itemInfo.tokenId].itemType > 0, "Token does not exist");

     tokenDetails[itemInfo.tokenId] = Item(itemInfo.itemType, itemInfo.strength, itemInfo.level);
}

```
