mergeInto(LibraryManager.library, {

    GetUniqueID: function (storageKey) {
        var response = window.UniqueIdManager.getId(
            Pointer_stringify(storageKey)
        );
        var bufferSize = lengthBytesUTF8(response) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(response, buffer, bufferSize);
        return buffer;
    }
});