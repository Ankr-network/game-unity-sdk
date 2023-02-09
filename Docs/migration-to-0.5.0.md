# Wallet Connect breaking changes migration guide

In this release we are adding meaningful simplicity, flexibility and extendability improvements into the SDK's wallet connect protocol implementation. These improvements introduced some breaking changes and this guide is intended to assist in the migration to 0.5.

### 1. `WalletConnect` **class is no longer a** `MonoBehaviour`

Previous implementation of `WalletConnect` relied on calling Awake or Start method to launch a connection with a Bridge server. In version 0.5.0 `WalletConnect` is no longer a `MonoBehaviour` so the user has a flexibility to instantiate it and call `Connect()` method whenever it is convenient to do. 

In order to migrate you would have to call Connect() on your own and also make sure that methods `OnApplicationPause()`, `Update()` and `Quit()` are called in a similar way they are being called by Unity events in `WalletConnectUnityMonoAdapter`. Alternatively you can use `WalletConnectUnityMonoAdapter` added to your scene and add a `WalletConnect` instance to `WalletConnectUnityMonoAdapter` in runtime using `TryAddObject()` method. 

### 2. **All serialized setting fields of** `WalletConnect` `MonoBehaviour` **are moved to** `WalletConnectSettings` **scriptable object**

If you have any custom settings within the prefab just move them to a settings scriptable object or alternatively create your own instance of same scriptable object class.

### 3. **Session instance is no longer accessible to a** `WalletConnect` **user**

All public properties and methods of `WalletConnectUnitySession` and its subclasses are replaced with `WalletConnect` class public properties and methods to be used directly. 

If you need to check if session instance was created you can replace a null check to a status check like this:

`_walletConnect.Session != null` is equivalent to `_walletConnect.Status != WalletConnectStatus.Uninitialized`

### 4. `WalletConnectStatus` **enum introduced**

Null checks of `WalletConnectUnitySession` instance as well as few of its bool flags are replaced with a single enum tracking status of the Wallet Connect session.

`WalletConnectStatus` enum has 6 values: Uninitialized, DisconnectedNoSession, DisconnectedSessionCached, TransportConnected, SessionRequestSent, WalletConnected

Initially the status is Uninitialized and is set to `DisconnectedNoSession ` or `DisconnectedSessionCached` right after session instance creation depending on if session was cached locally or not. If status is `DisconnectedSessionCached` then it transitions to `WalletConnected` right after the internal transport layer web socket connection is opened. If status is `DisconnectedNoSession` then after `Connect()` method is called it transitions to `TransportConnected` when transport layer web socket connection is opened. `TransportConnected` transitions to `SessionRequestSent` once `WalletConnect` sent a request to a Bridge server. After the user approves the connection in his wallet app (i.e. Metamask) WalletConnect gets a response from Bridge server and transitions status to `WalletConnected`.

For session flags:

`_walletConnect.Session.TransportConnected` is equivalent to `_walletConnect.Status.IsEqualOrGreater(WalletConnectStatus.TransportConnected)`

`_walletConnect.Session.ReadyForUserPrompt` is equivalent to `_walletConnect.Status == WalletConnectStatus.SessionRequestSent`

`_walletConnect.Session.Connected` is equivalent to `_walletConnect.Status == WalletConnectStatus.WalletConnected`

### 5. **All** `WalletConnect` **and session instance events are replaced with a single generic event** `WalletConnect.SessionStatusUpdated` 

This event has a single argument of a WalletConnectTransitionBase class while an actual object class depends on particular status transition that happens for a WalletConnect status. Client of this event is supposed to determine the actual type of an object to understand clearly which transition happens at the moment. It can be done with _is_ operator or _switch-case_ on an event argument object.

For transition object types:

`SessionCreatedTransition` is equivalent to `_walletConnect.Session.OnSessionCreated`
`TransportConnectedTransition` is equivalent to `_walletConnect.Session.OnTransportConnect`
`SessionRequestSentTransition` is equivalent to `_walletConnect.Session.OnReadyForUserPrompt`
`WalletConnectedTransition` is equivalent to `_walletConnect.Session.OnSessionCreated`
`WalletDisconnectedTransition` is equivalent to `_walletConnect.Session.OnSessionDisconnect`

### 6. **All C# Tasks are replaced with UniTasks within WalletConnect where possible**