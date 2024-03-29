# AnkrSDK / MirageSDK version 0.6.0 migration guide

In this release, we are adding a WalletConnect 2.0 protocol implementation based on [WalletConnectSharp](https://github.com/WalletConnect/WalletConnectSharp) implementation with some additions intended to support previously developed functionality. You can inspect the additional WalletConnectSharp changes in the [WalletConnectSharp-Fork](https://github.com/Ankr-network/WalletConnectSharp-Fork) repo main branch. In order to make sure WalletConnect 2.0 implementation can support a similar interface as WalletConnect 1.0 implementation we had to bring some breaking changes.

### 1. `WalletConnect.Connect()` method does not return the `WCSessionData` object anymore.

Since WalletConnect 2.0 implementation is not compatible with WCSessionData class which contains certain fields not compatible with WalletConnect 2.0 it was removed from a return value of the `Connect()` method. If you want to use WalletConnect 1.0 implementation with the 0.6.0 version you can still access the values of `WCSessionData` like this:

Let there be a `wcSessionData` object reference of the `WCSessionData` type and a `walletConnect` object reference of the `WalletConnect` type. This table shows which field corresponds to which between these two classes. 

| 0.5.x | 0.6.x |
| --- | --- |
| `wcSessionData.peerId` | `walletConnect.PeerId` | 
| `wcSessionData.peerMeta` | `walletConnect.WalletMetadata` | 
| `wcSessionData.chainId` | `walletConnect.ChainId` | 
| `wcSessionData.networkId` | `walletConnect.NetworkId` | 
| `wcSessionData.accounts` | `walletConnect.Accounts` | 

### 2. The `AppEntry` class was renamed to `WalletEntry`

Since this class is only related to wallet apps in every context of its usage it was renamed to WalletEntry. 

### 3. The `SettingsType` field is removed from the `IWalletConnectable` interface

This field is not required to be in this interface since it is possible to initialize all `IWalletConnectable` implementations without using a Type object for settings scriptable object.

### 4. `IWalletConnectCommunicator` is separated into two interfaces `IWalletConnectCommunicator` and `IWalletConnectGenericRequester`

`IWalletConnectGenericRequester` is intended to be used only within the SDK `WalletConnectClient` class. The `IWalletConnectCommunicator.Status` property was replaced with an `IWalletConnectGenericRequester.CanSendRequests` bool property determining when it is allowed to send a request with it.

The rest of the methods of `IWalletConnectCommunicator` stay the same except the 'Send' method

### 5. `IWalletConnectCommunicator.Send` generic method properties are now restricted to `IIdentifiable` and `IErrorHolder` interfaces

This is done to make both `WalletConnect` and `WalletConnect2` comply with the same `IWalletConnectCommunicator` interface.

### 6. AnkrSDK renamed to MirageSDK

Since the our web3 gaming platform name is Mirage we decided to rename this SDK to MirageSDK. Which leads to renaming of all of the namespaces and class names containing "Ankr" substring in them.
