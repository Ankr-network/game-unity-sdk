# AnkrSDK version 0.5.0 migration guide

In this release we are adding meaningful simplicity, flexibility and extendability improvements into the SDK's wallet connect protocol implementation. These improvements introduced some breaking changes and this guide is intended to assist in the migration to 0.5.

### 1. `WalletConnect` **class is no longer a** `MonoBehaviour`

Previous implementation of `WalletConnect` relied on calling Awake or Start method to launch a connection with a Bridge server. In version 0.5.0 `WalletConnect` is no longer a `MonoBehaviour` so the user has a flexibility to instantiate it and call `Connect()` method whenever it is convenient to do. 

In order to migrate you would have to call Connect() on your own and also make sure that methods `OnApplicationPause()`, `Update()` and `Quit()` are called in a similar way they are being called by Unity events in `WalletConnectUnityMonoAdapter`. 

```csharp
public class WalletConnectMonoBehaviourUsageExample : MonoBehaviour
{
	private WalletConnect _walletConnect;

	private async void Awake()
	{
		_walletConnect = new WalletConnect();
		var settings = Resources.Load<WalletConnectSettingsSO>("WalletConnectSettings");
		_walletConnect.Initialize(settings);
		await _walletConnect.Connect();
	}

	private void Update()
	{
		_walletConnect?.Update();
	}

	private async void OnApplicationPause(bool pause)
	{
		if (_walletConnect != null)
		{
			await _walletConnect.OnApplicationPause(pause);
		}
	}

	private async void OnApplicationQuit()
	{
		if (_walletConnect != null)
		{
			await _walletConnect.Quit();
		}
	}

	private async void OnDestroy()
	{
		if (_walletConnect != null)
		{
			await _walletConnect.Quit();
		}
	}
}
```

Alternatively you can use `WalletConnectUnityMonoAdapter` added to your scene and add a `WalletConnect` instance to `WalletConnectUnityMonoAdapter` in runtime using `TryAddObject()` method. If you choose this option, make sure WalletConnectUnityMonoAdapter is a component of an active game object.

```csharp
public class WalletConnectUsageExample
{
	public async void LaunchWalletConnect(WalletConnectUnityMonoAdapter adapter)
	{
		var walletConnect = new WalletConnect();
		var settings = Resources.Load<WalletConnectSettingsSO>("WalletConnectSettings");
		walletConnect.Initialize(settings);
		connectAdapter.Clear();
		adapter.TryAddObject(walletConnect);
		await walletConnect.Connect();
	}
}
```

Alternatively you can use `ConnectProvider` class to do the same thing in short. If you choose this option, make sure WalletConnectUnityMonoAdapter is a component of an active game object.

```csharp
public class ConnectProviderUsageExample
{
	public async void LaunchWalletConnect()
	{
		var walletConnect = ConnectProvider<WalletConnect>.GetConnect();
		await walletConnect.Connect();
	}
}
```

### 2. **All serialized setting fields of** `WalletConnect` `MonoBehaviour` **are moved to** `WalletConnectSettings` **scriptable object**

If you have any custom settings within the prefab just move them to a settings scriptable object or alternatively create your own instance of same scriptable object class.

### 3. **Session instance is no longer accessible to a** `WalletConnect` **user**

All public properties and methods of `WalletConnectUnitySession` and its subclasses are replaced with `WalletConnect` class public properties and methods to be used directly. 

If you need to check if session instance was created you can replace a null check to a status check like this:

| 0.4.x | 0.5.x |
| --- | --- |
| `_walletConnect.Session != null` | `_walletConnect.Status != WalletConnectStatus.Uninitialized` | 

### 4. `WalletConnectStatus` **enum introduced**

Null checks of `WalletConnectUnitySession` instance as well as few of its bool flags are replaced with a single enum tracking status of the Wallet Connect session.

`WalletConnectStatus` enum has 6 values: Uninitialized, DisconnectedNoSession, DisconnectedSessionCached, TransportConnected, SessionRequestSent, WalletConnected

Initially the status is `Uninitialized`. If `WalletConnect.Connect()` method is called then status is set to `DisconnectedNoSession` if there is no session cached locally. If session was cached locally from previous usages then status is changed to `DisconnectedSessionCached`. If status is `DisconnectedSessionCached` then it transitions to `WalletConnected` right after the internal transport layer web socket connection is opened. If status is `DisconnectedNoSession` then it transitions to `TransportConnected` when transport layer web socket connection is opened. `TransportConnected` transitions to `SessionRequestSent` once `WalletConnect` sends a request to a Bridge server. After a user approves the connection in her wallet app (i.e. Metamask) WalletConnect gets a response from Bridge server and transitions status to `WalletConnected`.

For session flags replace these with status enum value usages:

| 0.4.x | 0.5.x |
| --- | --- |
| `_walletConnect.Session.TransportConnected` | `_walletConnect.Status.IsEqualOrGreater(WalletConnectStatus.TransportConnected)` | 
| `_walletConnect.Session.ReadyForUserPrompt` | `_walletConnect.Status == WalletConnectStatus.SessionRequestSent` | 
| `_walletConnect.Session.Connected` | `_walletConnect.Status == WalletConnectStatus.WalletConnected` | 



### 5. **All** `WalletConnect` **and session instance events are replaced with a single generic event** `WalletConnect.SessionStatusUpdated` 

This event has a single argument of a WalletConnectTransitionBase class while an actual object class depends on particular status transition that happens for a WalletConnect status. Client of this event is supposed to determine the actual type of an object to understand clearly which transition happens at the moment. It can be done with _is_ operator or _switch-case_ on an event argument object.

Here is a table of equivalence for transition object types:

| 0.4.x | 0.5.x |
| --- | --- |
| `_walletConnect.Session.OnSessionCreated` | `_walletConnect.SessionStatusUpdated` with argument type `SessionCreatedTransition` | 
| `_walletConnect.Session.OnTransportConnect` | `_walletConnect.SessionStatusUpdated` with argument type `TransportConnectedTransition` | 
| `_walletConnect.Session.OnReadyForUserPrompt` | `_walletConnect.SessionStatusUpdated` with argument type `SessionRequestSentTransition` | 
| `_walletConnect.Session.OnSessionConnect` | `_walletConnect.SessionStatusUpdated` with argument type `WalletConnectedTransition` | 
| `_walletConnect.Session.OnSessionDisconnect` | `_walletConnect.SessionStatusUpdated` with argument type `WalletDisconnectedTransition` | 

### 6. **All C# Tasks are replaced with UniTasks within WalletConnect where possible**
