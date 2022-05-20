mergeInto(LibraryManager.library, {
    SignMessage: function (id, payload) {
        window.transactionHandler.signMessage(
            Pointer_stringify(id),
            Pointer_stringify(payload)
        );
    },

    SendTransaction: function (id, payload) {
        window.transactionHandler.sendTransaction(
            Pointer_stringify(id),
            Pointer_stringify(payload)
        );
    },

    GetContractData: function (id, payload) {
        window.transactionHandler.getContractData(
            Pointer_stringify(id),
            Pointer_stringify(payload)
        );
    },

    EstimateGas: function (id, payload) {
        window.transactionHandler.estimateGas(
            Pointer_stringify(id),
            Pointer_stringify(payload)
        );
    },

    GetAddresses: function (id) {
        window.transactionHandler.getAccounts(
            Pointer_stringify(id)
        );
    },

    GetTransaction: function (id, transactionHash) {
        window.transactionHandler.getTransaction(
            Pointer_stringify(id),
            Pointer_stringify(transactionHash)
        );
    },

    GetTransactionReceipt: function (id, transactionHash) {
        window.transactionHandler.getTransactionReceipt(
            Pointer_stringify(id),
            Pointer_stringify(transactionHash)
        );
    },

    SwitchChain: function (id, networkData) {
        window.transactionHandler.switchChain(
            Pointer_stringify(id),
            Pointer_stringify(networkData)
        );
    },

    GetEvents: function (id, filters) {
        window.transactionHandler.getEvents(
            Pointer_stringify(id),
            Pointer_stringify(filters)
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