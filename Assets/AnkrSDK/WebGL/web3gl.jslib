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

    GetAddresses: function (id) {
        window.transactionHandler.getAccounts(
            Pointer_stringify(id)
        );
    },

    GetResponses: function () {
        var responses = window.messageQueue.messages;
        var bufferSize = lengthBytesUTF8(responses) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(responses, buffer, bufferSize);
        return buffer;
    }
});