# AnkrSDK WalletConnect 2.0 migration guide

In the SDK version 0.6.0 we are introducing WalletConnect 2.0 implementation. It is represented by `WalletConnect2` class having partially identical and partially similar interface as `WalletConnect` class. This extension also includes additional Unity example scenes analogous to previously added WalletConnect example scenes.

This guide explains the differences in the interface of two implementations

### 1. Status enum changed

`WalletConnect.Status` property returns an enum value of `WalletConnectStatus` type while `WalletConnect2.Status` property returns an enum value of `WalletConnect2Status` type.  `WalletConnect2Status` has 4 values `Uninitialized`, `Disconnected`, `ConnectionRequestSent` and `WalletConnected`. These correspond to the values of `WalletConnectStatus` with the same numeric value. `TransportConnected` and `DisconnectedSessionCached` value is deprecated for `WalletConnect2`.

### 2. Status transition event changed

`WalletConnect2.SessionStatusUpdated` event has a different argument type now, `WalletConnect2TransitionBase` instead of `WalletConnectTransitionBase` with separate set of subclasses corresponding to each possible transition.


### 3. `WalletConnect2` no longer implements `IUpdatable` interface

`IUpdatable` is not implemented for `WalletConnect2` since this implementation does not have to be ticked on per-frame basis in order to function and trigger status transition events.

