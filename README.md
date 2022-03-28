
<h3 align="center">Unity SDK</h3>

  <p align="center">
The Ankr Unity SDK provides an easy way to interact with Web3 and to work with contracts deployed on the blockchain. As a plugin it easily integrates with MetaMask on Android and iOS.
    <br />
    <a href="https://github.com/Ankr-network/game-unity-demo">View Demo</a>
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#install-sdk">Install SDK</a>
    </li>
	<li>
      <a href="#what's-in-the-sdk">What's in the SDK</a>
    </li>
    <li><a href="#getting-started">Getting Started</a></li>
	<ul>
        <li><a href="#prequisites">Prerequisites</a></li>
        <li><a href="#use-cases">Use Cases</a></li>
	</ul>
    <li>
      <a href="#01-connect-web3-wallet">Connect Web3 Wallet</a></li>
	  <a href="#02-perform-updates-to-nfts">Perform Updates to NFTs</a>
      <ul>
        <li><a href="#signing-transactions">Signing Transactions</a></li>
        <li><a href="#sending-transactions">Sending Transactions</a></li>
        <li><a href="#getdata-method">GetData Method</a></li>
        <li><a href="#callmethod">CallMethod</a></li>
      </ul>    
    <li><a href="#current-erc-proposals">Current ERC Proposals</a></li>
	<li><a href="#view-full-examples">View Full Examples</a></li>
  </ol>
</details>

<!-- Installation -->

##  🏗 Install SDK

1. Download `AnkrSDK.unitypackage` from the latest [release](https://github.com/Ankr-network/game-unity-sdk/releases).

2. In your project, locate the 'Assets' folder. Move `AnkrSDK.unitypackage` into this folder.

3. 'Import all' to have access to all SDK capabilities.

## 👀 What's in the SDK

The SDK is designed to make it super easy to get started with Game development by enabling connection and interaction across different blockchains. 

 * Contains a huge range of classes, plugins and example scripts for a variety of use cases. 

* Nethereum libraries provide support for web requests using RPC over Http. 

* Ankr RPC network infrastructure provides fast and easy connection to multiple chains. 


## Getting started

### 🧰 Prerequisites

1. Smart Contracts must already be deployed on the blockchain. You have the Smart Contract addresses and ABI.

###  📌 Use Cases

This help content focuses on the following use cases. 

1. Connecting to a Web3 Wallet (MetaMask) and Authenticating a user

2. Performing Updates to NFTs by interacting with the blockchain and calling smart functions.


<!-- 
1. Examples Scripts, 
*  The *Scripts* folder contains a wide range of useful scripts for initializing sessions, creating and managing Game NFTs and Scenes and authenticating sessions via a Web3 Wallet e.g. Metamask. 
`ContractMessage` scripts for retrieving information for and managing NFTs  e.g. Game Characters on the blockchain.  ERC-721 and ERC-1155 
`DTO` scripts for handling TransferObject events and accessing logs.
`ERC-20`folder contains example script for minting new NFTs e.g. characters
`ERC-721` folder contains example scripts for managing ownership of NFTs.
`WearableNFTExample` folder contains example scripts for minting and managing NFT character apparel e.g. hats. 
`ConnectionController` and `CustomSceneManager` ensure ongoing authentication via MetaMask and displaying game scenes. 
to allow smart contract interfacing with the for building NFTs to the Blockchain.  scripts to ERC721 and ERC1155 scriptsScripts that can be used  -->

## 👝 01 Connect Web3 Wallet

Connecting to an Ethereum i.e. MetaMask wallet provides a link between a wallet address and a user's game account.
You have to connect your wallet via WalletConnect.
There is a number of ways which you can get a session key from your wallet:
- Connect with QRCode using **QRCodeImage** component. 
- Connect using WalletConnect.Instance.OpenMobileWallet. 

Both of these methods will generate a linking url which will create a request in your wallet to connect.
If you accept to connect a session key will be saved in PlayerPrefs for future use.


1. Create an instance of a `AnkrSDKWrapper` class via `AnkrSDKWrapper.GetSDKInstance(...)` method after successful connection to your wallet

```c#		
var ankrSdk = AnkrSDKWrapper.GetSDKInstance("<ethereum node url>");
```

Inside (AnkrSDK/Examples/UseCases/LinkingAccountWallet) is an example script demonstrating how to link a crypto wallet (MetaMask) to a player account.

## 🚀 02 Perform Updates to NFTs

Making updates to the NFT e.g. adding a red hat to a character requires signing and sending a transaction.

### Signing Transactions

All updates are transactions that must be signed via a prompt from MetaMask.

#### Sign message

Login via WalletConnect is required to sign the message.

- Call the `AnkrSignatureHelper.Sign` to trigger Signing via MetaMask. 

```c#
string message = "Hi I am a message !"
string signature = await AnkrSignatureHelper.Sign(message);
```

- `AnkrSignatureHelper.Sign(string)` returns the signature.

#### Verify the user signed message

```c#
AnkrSignatureHelper.CheckSignature(message, signature);
```

### Sending Transactions

There are two ways to make update transactions. 
Using the **GetData** method and the **CallMethod**

#### GetData Method

Use the `GetData` method to retrieve information from the blockchain. (READ functions do NOT require mining.). Other non-standard `Get` functions are also available

These methods require

* Contract Address
* ABI

The following extract is an example usage of the **GetData** method to retrive information about an NFT:

```c#
private async UniTask<BigInteger> GetHat(BigInteger tokenID)
{
	var getHatMessage = new GetHatMessage
	{
		CharacterId = tokenID.ToString()
	};
	var hatId = await _gameCharacterContract.GetData<GetHatMessage, BigInteger>(getHatMessage);

	UpdateUILogs($"Hat Id: {hatId}");

	return hatId;
}
```


#### CallMethod

Use the `CallMethod` to write new information to the blockchain. These methods utilize gas. 

The following extract is an example of how a `CallMethod` is used to update an NFT: 

```c#
public async void UpdateNFT()
{
	// 1) Request nft parameters and signature for parameters
	var info = await RequestPreparedParams(0);
	// 2) Call method that check signature and update nft
	var receipt = await _contract.CallMethod("updateTokenWithSignedMessage", new object[] { info });

	Debug.Log($"Receipt: {receipt}");
}
```

#### Handle transaction events

There is a `ITransactionEventHandler` argument which provides an access to keypoints of transaction flow. <br>

Implement `ITransactionEventHanlder` and pass the instance as an argument to be able to intercept those events. <br>
There are pre-crafted implementations: <br>
Allows you to subscribe to events <br>
```c#
public class TransactionEventDelegator : ITransactionEventHandler
```
Base class for you to implement, so you don't have to impement each callback. <br>
```c#
public class TransactionEventHanlder : ITransactionEventHandler
```

## SDK Detailed description
### IAnkrSDK 
After you finished connecting your wallet via WalletConnect you can access SDK Functionallity. First of all you should get an sdk Instance:
```c#
var ankrSDK = AnkrSDKWrapper.GetSDKInstance("<etherium_node_url>");
```
#### EthHandler
You can get EthHandler via get-only property from sdk instance.
```c#
_eth = ankrSDK.Eth;
```
#### IContract
IContract is a contract handler which gives you an easier access to web3, by handling low-level transformations.
```c#
_contract = ankrSDKWrapper.GetContract("<contractAddress>", "<contractABI>");
```

After you have IContract instance for your smart-contract you are now eligible to work with your contract deployed on blockchain
##### CallMethodAsync
```c#
Task<string> CallMethodAsync(string methodName, object[] arguments = null,
	string gas = null, string gasPrice = null,
	string nonce = null);
```
Sends a message via socket-established connection via json-rpc. Method name is "eth_sendTransaction"
Input:
- methodName: contract method name to call. Contract ABI should contain methodName.
- arguments: an array of arguments for contract call. Contract ABI should contain ABIType for passed arguments.
- gas: The amount of gas to use for the transaction (unused gas is refunded). 
- gasPrice: The price of gas for this transaction in wei,
- nonce: Integer of the nonce. This allows to overwrite your own pending transactions that use the same nonce.

Output:
- returns transaction hash

Example:
```c#
_contract.CallMethodAsync(rentOutMethodName, 
	new object[]
		{
			renter,
			tokenId,
			expiresAt
		})
```
##### Web3SendMethodAsync
```c#
Task Web3SendMethodAsync(string methodName, object[] arguments,
			string gas = null, string gasPrice = null, string nonce = null,
			ITransactionEventHandler eventHandler = null);
```
Input:
- methodName: contract method name to call. Contract ABI should contain methodName.
- arguments: an array of arguments for contract call. Contract ABI should contain ABIType for passed arguments.
- gas: The amount of gas to use for the transaction (unused gas is refunded). 
- gasPrice: The price of gas for this transaction in wei,
- nonce: Integer of the nonce. This allows to overwrite your own pending transactions that use the same nonce.
- eventHandler: event handler which allows you to track every step of your transaction.

Output:
- void

To get a receipt using **Web3SendMethodAsync** you can subscribe to ITransactionEventHandler 
```c#
void ReceiptReceived(TransactionReceipt receipt);
```
**ITransactionEventHandler** callback:
- Transaction Begin - is called right after transaction input was created
- Transaction End - is called after transaction was requested, but still pending
- Transaction Hash - is called in case if task was not faulted and hash was received
- Transaction Error - is called if there was a error during transaction execution
- Transaction Receipt - receipt is requested in case if transaction was successful via EthHandler and is passed to Receipt callback.

##### GetAllChanges
```c#
Task<List<EventLog<TEvDto>>> GetAllChanges<TEvDto>(EventFilterData evFilter) where TEvDto : IEventDTO, new()
```
Input:
- evFilter:EventFilterData is a simple data holder which allows you to filter events.
Output
- List of event logs filled with supplied DTO.

EventFilterData allows you to filter event by topics (3 max) and "To Block" with "From Block"

##### GetData
```c#
Task<TReturnType> GetData<TFieldData, TReturnType>(TFieldData requestData = null)
			where TFieldData : FunctionMessage, new()
```
Queries a request to get data from contract

##### EstimateGas
Makes a call or transaction, which won't be added to the blockchain and returns the used gas, which can be used for
estimating the used gas.
```c#
Task<HexBigInteger> EstimateGas(string methodName, object[] arguments = null,
			string gas = null, string gasPrice = null,
			string nonce = null)
```
Input:
- methodName: contract method name to estimate. Contract ABI should contain methodName.
- arguments: an array of arguments for contract call. Contract ABI should contain ABIType for passed arguments.
- gas: The amount of gas to use for the transaction (unused gas is refunded). 
- gasPrice: The price of gas for this transaction in wei,
- nonce: Integer of the nonce. This allows to overwrite your own pending transactions that use the same nonce.

Output:
- estimated gas price for transaction

## Current ERC Proposals

We have two ERC proposals.  
[ERC-4884  Rentable NFT Standard](https://github.com/Ankr-network/game-smart-contract-example/blob/master/ERC/rentable-nft.md)
[ERC-4911  Composability Extension For ERC-721 Standard](https://github.com/Ankr-network/game-smart-contract-example/blob/master/ERC/composable-nft.md)


<p align="right">(<a href="#top">back to top</a>)</p>


## View Full Examples

For full examples:

View 
[ERC20 token example](https://github.com/Ankr-network/game-unity-sdk/blob/master/Assets/AnkrSDK/Examples/Scripts/ERC20Example/ERC20Example.cs) and
[ERC721 token example](https://github.com/Ankr-network/game-unity-sdk/blob/master/Assets/AnkrSDK/Examples/Scripts/ERC721Example/ERC721Example.cs)
