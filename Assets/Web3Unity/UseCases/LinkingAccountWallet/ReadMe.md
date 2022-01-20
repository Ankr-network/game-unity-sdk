# Bind wallet with account

This is an example for one of the use cases for this sdk : the linking of a crypto wallet to a player account.

### Steps

To start this use case we need to make instance of `Web3` class and call `Initialize` method after login in metamask

```c#
string provider_url = "<ethereum node url>";
		
Web3 web3 = new Web3(provider_url);
web3.Initialize();
```
---

To prove ownership of crypto address player should sign arbitrary string with not zero length. We recommend using uuid strings.<br>
3rd party wallet will provide sign functionality. To start this step call method `Sign`.

```c#
string message = "Hi I am a message !"
string signature = await web3.Sign(message);
```

`Web3.Sign(string)` will return the the signature.

---
To continue that process you need to deploy [backend side](https://github.com/mirage-xyz/mirage-go-sdk). Please proceed through deploy steps in [readme](https://github.com/mirage-xyz/mirage-go-sdk/blob/main/README.md).
To verify user you need to call rest api method POST `/account/verification/address` with payload

```json
{
  "message": "Hi I am a message !", // your message
  "signature":"0x..." // result of Web3.Sign()
}
```

This method will get address of user from signature that you can write to database.

```go
...
sigPublicKey := getAddrFromSign(input.Signature, data)
address := string(sigPublicKey);
// add address to a database
...

```
