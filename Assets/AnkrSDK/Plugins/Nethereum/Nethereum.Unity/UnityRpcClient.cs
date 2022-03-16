using System;
using System.Text;
using Nethereum.Unity.RpcModel;
using Nethereum.JsonRpc.Client;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RpcError = Nethereum.JsonRpc.Client.RpcError;
using RpcRequest = Nethereum.JsonRpc.Client.RpcRequest;
using Nethereum.RPC.Eth.Transactions;


namespace Nethereum.JsonRpc.UnityClient
{
    public class UnityRpcClient<TResult>:UnityRequest<TResult>, IClientRequestHeaderSupport
    {
        private readonly string _url;

        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();
        
        public UnityRpcClient(string url, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (jsonSerializerSettings == null)
                jsonSerializerSettings = DefaultJsonSerializerSettingsFactory.BuildDefaultJsonSerializerSettings();
            this._url = url;
            this.SetBasicAuthenticationHeaderFromUri(new Uri(url));
            //check for nulls
            JsonSerializerSettings = jsonSerializerSettings;
        }

        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        private RpcResponseException HandleRpcError(RpcResponse response)
        {
            if (response.HasError)
                return new RpcResponseException(new RpcError(response.Error.Code, response.Error.Message,
                    response.Error.Data));
            return null;
        }

        public IEnumerator SendRequest(RpcRequest request) 
        {
            var requestFormatted = new Unity.RpcModel.RpcRequest(request.Id, request.Method, request.RawParameters);
        
            var rpcRequestJson = JsonConvert.SerializeObject(requestFormatted, JsonSerializerSettings);
            var requestBytes = Encoding.UTF8.GetBytes(rpcRequestJson);
            using (var unityRequest = new UnityWebRequest(_url, "POST")) 
            { 
                var uploadHandler = new UploadHandlerRaw(requestBytes);
                unityRequest.SetRequestHeader("Content-Type", "application/json");
                uploadHandler.contentType= "application/json";
                unityRequest.uploadHandler = uploadHandler;

                unityRequest.downloadHandler = new DownloadHandlerBuffer();

                if (RequestHeaders != null)
                {
                    foreach (var requestHeader in RequestHeaders)
                    {
                        unityRequest.SetRequestHeader(requestHeader.Key, requestHeader.Value);
                    }
                }

                yield return unityRequest.SendWebRequest();

                    if (unityRequest.error != null)
                    {
                        this.Exception = new Exception(unityRequest.error);
    #if DEBUG
                        Debug.Log(unityRequest.error);
    #endif
                    }
                    else
                    {
                        try
                        {
                            byte[] results = unityRequest.downloadHandler.data;
                            var responseJson = Encoding.UTF8.GetString(results);
    #if DEBUG
                            Debug.Log(responseJson);
    #endif
                            var responseObject = JsonConvert.DeserializeObject<RpcResponse>(responseJson, JsonSerializerSettings);
                            this.Result = responseObject.GetResult<TResult>(true, JsonSerializerSettings);
                            this.Exception = HandleRpcError(responseObject);
                        }
                        catch (Exception ex)
                        {
                            this.Exception = new Exception(ex.Message);
    #if DEBUG
                            Debug.Log(ex.Message);
    #endif
                        }
                }
            }
        }
    }
}  
