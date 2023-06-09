mergeInto(LibraryManager.library, {

    CreateProvider: function (id, payload) {
        window.ProviderFabric.createProvider(
            UTF8ToString(id),
            UTF8ToString(payload)
        );
    },

    GetWalletsStatus: function (id) {
        window.ProviderFabric.getWalletsStatus(
            UTF8ToString(id)
        );
    },
    
    SignMessage: function (id, payload) {
        window.WalletProvider.signMessage(
            UTF8ToString(id),
            UTF8ToString(payload)
        );
    },

    SendTransaction: function (id, payload) {
        window.WalletProvider.sendTransaction(
            UTF8ToString(id),
            UTF8ToString(payload)
        );
    },

    GetContractData: function (id, payload) {
        window.WalletProvider.getContractData(
            UTF8ToString(id),
            UTF8ToString(payload)
        );
    },

    EstimateGas: function (id, payload) {
        window.WalletProvider.estimateGas(
            UTF8ToString(id),
            UTF8ToString(payload)
        );
    },

    GetAddresses: function (id) {
        window.WalletProvider.getAccounts(
            UTF8ToString(id)
        );
    },

    GetTransaction: function (id, transactionHash) {
        window.WalletProvider.getTransaction(
            UTF8ToString(id),
            UTF8ToString(transactionHash)
        );
    },

    GetTransactionReceipt: function (id, transactionHash) {
        window.WalletProvider.getTransactionReceipt(
            UTF8ToString(id),
            UTF8ToString(transactionHash)
        );
    },

    AddChain: function (id, networkData) {
        window.WalletProvider.addChain(
            UTF8ToString(id),
            UTF8ToString(networkData)
        );
    },
    
    SwitchChain: function (id, networkData) {
        window.WalletProvider.switchChain(
            UTF8ToString(id),
            UTF8ToString(networkData)
        );
    },

    GetEvents: function (id, filters) {
        window.WalletProvider.getEvents(
            UTF8ToString(id),
            UTF8ToString(filters)
        );
    },

    RequestChainId: function (id) {
        window.WalletProvider.getChainId(
            UTF8ToString(id)
        );
    },

    CallMethod: function (id, callObject) {
        window.WalletProvider.callMethod(
            UTF8ToString(id),
            UTF8ToString(callObject)
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