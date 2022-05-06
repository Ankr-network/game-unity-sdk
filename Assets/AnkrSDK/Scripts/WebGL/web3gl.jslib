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

    GetResponses: function () {
        var responses = window.MQ.messages;
        var bufferSize = lengthBytesUTF8(responses) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(responses, buffer, bufferSize);
        return buffer;
    }
});