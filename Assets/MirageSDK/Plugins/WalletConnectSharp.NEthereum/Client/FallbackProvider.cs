using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MirageSDK.WalletConnectSharp.Core;
using Nethereum.JsonRpc.Client;

namespace MirageSDK.WalletConnectSharp.NEthereum.Client
{
    public class FallbackProvider : IClient
    {
        public static readonly string[] ValidMethods = WalletConnectProtocol.SigningMethods;
        
        private readonly IClient _fallback;
        private readonly IClient _primary;

        public FallbackProvider(IClient primary, IClient fallback)
        {
            _primary = primary;
            _fallback = fallback;
        }
        
        public Task SendRequestAsync(RpcRequest request, string route = null)
        {
            return ValidMethods.Contains(request.Method) ? _primary.SendRequestAsync(request, route) : _fallback.SendRequestAsync(request, route);
        }

        public Task SendRequestAsync(string method, string route = null, params object[] paramList)
        {
            return ValidMethods.Contains(method) ? _primary.SendRequestAsync(method, route, paramList) : _fallback.SendRequestAsync(method, route, paramList);
        }

        public RequestInterceptor OverridingRequestInterceptor { get; set; }
        public async Task<RpcRequestResponseBatch> SendBatchRequestAsync(RpcRequestResponseBatch rpcRequestResponseBatch)
        {
            var tasks = new List<Task>();
            foreach (var requestMessage in rpcRequestResponseBatch.GetRpcRequests())
            {
                var method = requestMessage.Method;
                var paramList = (object[])requestMessage.RawParameters;
                var task = ValidMethods.Contains(method) ? _primary.SendRequestAsync(method, route:null, paramList) : _fallback.SendRequestAsync(method, route:null, paramList);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            return rpcRequestResponseBatch;
        }

        public Task<T> SendRequestAsync<T>(RpcRequest request, string route = null)
        {
            return ValidMethods.Contains(request.Method) ? _primary.SendRequestAsync<T>(request, route) : _fallback.SendRequestAsync<T>(request, route);
        }

        public Task<T> SendRequestAsync<T>(string method, string route = null, params object[] paramList)
        {
            return ValidMethods.Contains(method) ? _primary.SendRequestAsync<T>(method, route, paramList) : _fallback.SendRequestAsync<T>(method, route, paramList);
        }
    }
}