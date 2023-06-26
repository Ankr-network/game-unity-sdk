mergeInto(LibraryManager.library, {
    EnableEthereumRpcClientCallback: async function (callback, fallback) {
        try {
            const accounts = await ethereum.request({ method: 'eth_requestAccounts' });
            ethereum.autoRefreshOnNetworkChange = false;

            var bufferSize = lengthBytesUTF8(accounts[0]) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(accounts[0], buffer, bufferSize);
            Module.dynCall_vi(callback, buffer);

            return buffer;
        } catch (error) {

            var errorMessage = error.message;
            var bufferSize = lengthBytesUTF8(errorMessage) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(errorMessage, buffer, bufferSize);
            Module.dynCall_vi(fallback, buffer);

            return null;
        }
    },
    GetChainIdRpcClientCallback: async function(callback, fallback) {
        try {

            const chainId = await ethereum.request({ method: 'eth_chainId' });
            var bufferSize = lengthBytesUTF8(chainId) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(chainId, buffer, bufferSize);
            Module.dynCall_vi(callback, buffer);

        } catch (error) {
            var errorMessage = error.message;
            var bufferSize = lengthBytesUTF8(errorMessage) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(errorMessage, buffer, bufferSize);
            Module.dynCall_vi(fallback, errorMessage);
            return null;
        }
    }
});