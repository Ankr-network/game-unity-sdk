# AnkrSDK WalletConnect 2.0 migration guide

In the SDK version 0.6.0, we are introducing WalletConnect 2.0 implementation. It is represented by the `WalletConnect2` class having a partially identical and partially similar interface to the `WalletConnect` class. This extension also includes additional Unity example scenes analogous to previously added WalletConnect example scenes.

This guide explains the differences in the interface of the two implementations

### 1. Status enum changed

`WalletConnect.Status` property returns an enum value of the `WalletConnectStatus` type while the `WalletConnect2.Status` property returns an enum value of the `WalletConnect2Status` type.  `WalletConnect2Status` has 4 values `Uninitialized`, `Disconnected`, `ConnectionRequestSent`, and `WalletConnected`. These correspond to the values of `WalletConnectStatus` with the same numeric value. `TransportConnected` and `DisconnectedSessionCached` value is deprecated for `WalletConnect2`.

### 2. The status transition event changed

`WalletConnect2.SessionStatusUpdated` event now has a different argument type: ' WalletConnect2TransitionBase` instead of `WalletConnectTransitionBase` with a separate set of subclasses corresponding to each possible transition.


### 3. `WalletConnect2` no longer implements the `IUpdatable` interface

`IUpdatable` is not implemented for `WalletConnect2` since this implementation does not have to be ticked on a per-frame basis in order to function and trigger status transition events.

### 4. `WalletConnect2` has a different disconnect method name

`WalletConnect2` does not have `CloseSession()` method but has `Disconnect()` to replace it
