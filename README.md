# Plugin for unity for creation 

This plugin integrates with metamask on android and iOS.<br>
Demo:<br>
[![Demo](https://img.youtube.com/vi/y9ceLv43kCI/0.jpg)](https://www.youtube.com/watch?v=y9ceLv43kCI)

## 1. Install SDK

1) Download ankr_web3.unitypackage from [latest release](https://github.com/Ankr-network/unity-web3/releases).
2) Drag and drop it to the Assets folder.
3) Import all 

## 2. Getting started

To start work with plugin
```c#
string provider_url = "<ethereum node url>";
		
Web3 web3 = new Web3(provider_url);
```

You need to call initialize method after login in metamask

```c#
web3.Initialize();
```

To get contract call GetContract

```c#
string abi = "...";
string contract_address = "0x...";

Contract contract = web3.GetContract(contract_address, abi);
```

**Contract methods**

*CallMethod(string methodName, object[] arguments, string gas = null)*<br>
Use for call methods of a contract deployed to blockchain.
Returns string with transaction hash.

```c#
string receipt = await contract.CallMethod("mint", new object[0]);
```
---
*GetTransactionInfo(string receipt)*<br>
Use for get transaction details like status, nonce and etc.
Returns transaction details

```c#
Transaction trx = await contract.GetTransactionInfo(receipt);
```
---
*GetData<FieldData, ReturnType>(FieldData requestData = null)*<br>
To get data from mappings and call contract methods that no need of mining use this method.

You need to prepare DTO class based on the arguments of a contract method.

Contract method:
```sol
function balanceOf(address owner) public view virtual override returns (uint256)
```

DTO:
```c#
[Function("balanceOf", "uint256")]
public class BalanceOfMessage : FunctionMessage
{
	[Parameter("address", "_owner", 1)]
	public virtual string Owner { get; set; }
}
```

Returns ReturnType<br>
Example:
```c#
BalanceOfMessage balanceOfMessage = new BalanceOfMessage()
{
	Owner = WalletConnect.ActiveSession.Accounts[0]
};
BigInteger balance = await contract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);
```
---
*GetAllChanges<EvDTO>()*<br>
Use for get events from a contract.

You need to prepare DTO class based on the fields of an event.

Event:
```sol
event Transfer(address indexed from, address indexed to, uint256 value);
```

DTO:
```c#
[Event("Transfer")]
public class TransferEventDTO : IEventDTO
{
	[Parameter("address", "_from", 1, true)]
	public string From { get; set; }

	[Parameter("address", "_to", 2, true)]
	public string To { get; set; }

	[Parameter("uint256", "_value", 3, false)]
	public BigInteger Value { get; set; }
}
```

Returns list of events

```c#
var events = await contract.GetAllChanges<TransferEventDTO>();

foreach (var ev in events)
{
	Debug.Log(ev.Event.Value);
}
```

For full examples please see [ERC20 token example](https://github.com/Ankr-network/unity-web3/blob/main/Assets/Web3Unity/Scripts/Example/ERC20Example.cs) and [ERC721 token example](https://github.com/Ankr-network/unity-web3/blob/main/Assets/Web3Unity/Scripts/Example/ERC721Example.cs)

## 3. Contracts interaction

In this plugin there are examples for ERC20 and ERC721 contracts integration, however, it can be easily expanded for any other types of contracts.

Contracts for this plugin are stored under `ExampleContracts` folder. Mint functions have no rights, so anyone can test this plugin. These contracts shouldn't be used in production.

Default web3 interactions are created using Nethereum package, the network and public RPC should be configured before using this plugin.

All contract interactions are stored under the `Contract.cs` file (events receiving, transaction hashes receiving, getting view data, sending and signing transactions)
	
## 4. Troubleshooting
Please pay attention that this plugin works only on Api Compatibility Level .NET 4.x
To setup that go to File > Build Settings > Player settings > Player > Api Compatibility Level and select .NET 4.x
