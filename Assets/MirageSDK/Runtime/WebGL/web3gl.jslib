mergeInto(LibraryManager.library, {

    CreateProvider: function (id, payload) {
        window.ProviderFabric.createProvider(
            Pointer_stringify(id),
            Pointer_stringify(payload)
        );
    },

    GetWalletsStatus: function (id) {
        window.ProviderFabric.getWalletsStatus(
            Pointer_stringify(id)
        );
    },
    
    SignMessage: function (id, payload) {
        window.WalletProvider.signMessage(
            Pointer_stringify(id),
            Pointer_stringify(payload)
        );
    },

    SendTransaction: function (id, payload) {
        window.WalletProvider.sendTransaction(
            Pointer_stringify(id),
            Pointer_stringify(payload)
        );
    },

    GetContractData: function (id, payload) {
        window.WalletProvider.getContractData(
            Pointer_stringify(id),
            Pointer_stringify(payload)
        );
    },

    EstimateGas: function (id, payload) {
        window.WalletProvider.estimateGas(
            Pointer_stringify(id),
            Pointer_stringify(payload)
        );
    },

    GetAddresses: function (id) {
        window.WalletProvider.getAccounts(
            Pointer_stringify(id)
        );
    },

    GetTransaction: function (id, transactionHash) {
        window.WalletProvider.getTransaction(
            Pointer_stringify(id),
            Pointer_stringify(transactionHash)
        );
    },

    GetTransactionReceipt: function (id, transactionHash) {
        window.WalletProvider.getTransactionReceipt(
            Pointer_stringify(id),
            Pointer_stringify(transactionHash)
        );
    },

    SwitchChain: function (id, networkData) {
        window.WalletProvider.switchChain(
            Pointer_stringify(id),
            Pointer_stringify(networkData)
        );
    },

    GetEvents: function (id, filters) {
        window.WalletProvider.getEvents(
            Pointer_stringify(id),
            Pointer_stringify(filters)
        );
    },

    GetChainId: function (id) {
        window.WalletProvider.getChainId(
            Pointer_stringify(id)
        );
    },

    CallMethod: function (id, callObject) {
        window.WalletProvider.callMethod(
            Pointer_stringify(id),
            Pointer_stringify(callObject)
        );
    },

    GetResponses: function () {
        var responses = window.MQ.messages;
        var bufferSize = lengthBytesUTF8(responses) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(responses, buffer, bufferSize);
        return buffer;
    }
});