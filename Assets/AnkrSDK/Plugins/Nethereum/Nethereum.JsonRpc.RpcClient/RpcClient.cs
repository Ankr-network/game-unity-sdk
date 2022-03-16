using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;

namespace Nethereum.JsonRpc.Client
{
    public class RpcClient : ClientBase
    {
        private const int NUMBER_OF_SECONDS_TO_RECREATE_HTTP_CLIENT = 60;
        public static int MaximumConnectionsPerServer { get; set; } = 20;
        private readonly AuthenticationHeaderValue _authHeaderValue;
        private readonly Uri _baseUrl;
        private readonly HttpClientHandler _httpClientHandler;
        private readonly ILog _log;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private volatile bool _firstHttpClient;
        private HttpClient _httpClient;
        private HttpClient _httpClient2;
        private bool _rotateHttpClients = true;
        private DateTime _httpClientLastCreatedAt;
        private readonly object _lockObject = new object();

        public RpcClient(Uri baseUrl, AuthenticationHeaderValue authHeaderValue = null,
            JsonSerializerSettings jsonSerializerSettings = null, HttpClientHandler httpClientHandler = null, ILog log = null)
        {
            _baseUrl = baseUrl;

            if (authHeaderValue == null)
            {
                authHeaderValue = BasicAuthenticationHeaderHelper.GetBasicAuthenticationHeaderValueFromUri(baseUrl);
            }

            _authHeaderValue = authHeaderValue;

            if (jsonSerializerSettings == null)
                jsonSerializerSettings = DefaultJsonSerializerSettingsFactory.BuildDefaultJsonSerializerSettings();
            
            _jsonSerializerSettings = jsonSerializerSettings;
            _httpClientHandler = httpClientHandler;
            _log = log;

#if NETCOREAPP2_1 || NETCOREAPP3_1 || NET5_0_OR_GREATER
            _httpClient = CreateNewHttpClient();
            _rotateHttpClients = false;
#else
            CreateNewRotatedHttpClient();
#endif
        }

        private static HttpMessageHandler GetDefaultHandler()
        {
            try
            {
#if NETSTANDARD2_0
                return new HttpClientHandler
                {
                    MaxConnectionsPerServer = MaximumConnectionsPerServer
                };
           
#elif NETCOREAPP2_1 || NETCOREAPP3_1 || NET5_0_OR_GREATER
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = new TimeSpan(0, NUMBER_OF_SECONDS_TO_RECREATE_HTTP_CLIENT, 0),
                    PooledConnectionIdleTimeout = new TimeSpan(0, NUMBER_OF_SECONDS_TO_RECREATE_HTTP_CLIENT, 0),
                    MaxConnectionsPerServer = MaximumConnectionsPerServer
                };
#else
                return null;
#endif
            }
            catch
            {
                return null;
            }
        }

        public RpcClient(Uri baseUrl, HttpClient httpClient, AuthenticationHeaderValue authHeaderValue = null,
           JsonSerializerSettings jsonSerializerSettings = null, ILog log = null)
        {
            _baseUrl = baseUrl;

            if (authHeaderValue == null)
            {
                authHeaderValue = BasicAuthenticationHeaderHelper.GetBasicAuthenticationHeaderValueFromUri(baseUrl);
            }

            _authHeaderValue = authHeaderValue;
            if (jsonSerializerSettings == null)
                jsonSerializerSettings = DefaultJsonSerializerSettingsFactory.BuildDefaultJsonSerializerSettings();
            _jsonSerializerSettings = jsonSerializerSettings;
            _log = log;
            InitialiseHttpClient(httpClient);
            _httpClient = httpClient;
            _rotateHttpClients = false;
        }

        protected override async Task<RpcResponseMessage> SendAsync(RpcRequestMessage request, string route = null)
        {
            var logger = new RpcLogger(_log);
            try
            {
                var httpClient = GetOrCreateHttpClient();
                var rpcRequestJson = JsonConvert.SerializeObject(request, _jsonSerializerSettings);
                var httpContent = new StringContent(rpcRequestJson, Encoding.UTF8, "application/json");
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(ConnectionTimeout);

                logger.LogRequest(rpcRequestJson);

                var httpResponseMessage = await httpClient.PostAsync(route, httpContent, cancellationTokenSource.Token).ConfigureAwait(false);
                httpResponseMessage.EnsureSuccessStatusCode();

                var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
                using (var streamReader = new StreamReader(stream))
                using (var reader = new JsonTextReader(streamReader))
                {
                    var serializer = JsonSerializer.Create(_jsonSerializerSettings);
                    var message =  serializer.Deserialize<RpcResponseMessage>(reader);

                    logger.LogResponse(message);
                    
                    return message;
                }
            }
            catch(TaskCanceledException ex)
            {
                 var exception = new RpcClientTimeoutException($"Rpc timeout after {ConnectionTimeout.TotalMilliseconds} milliseconds", ex);
                 logger.LogException(exception);
                 throw exception;
            }
            catch (Exception ex)
            {
                var exception = new RpcClientUnknownException("Error occurred when trying to send rpc requests(s): " + request.Method, ex);
                logger.LogException(exception);
                throw exception;
            }
        }

        private HttpClient GetOrCreateHttpClient()
        {
            if (_rotateHttpClients) //already created if not rotated
            {
                lock (_lockObject)
                {
                    var timeSinceCreated = DateTime.UtcNow - _httpClientLastCreatedAt;
                    if (timeSinceCreated.TotalSeconds > NUMBER_OF_SECONDS_TO_RECREATE_HTTP_CLIENT)
                        CreateNewRotatedHttpClient();
                    return GetClient();
                }
            }
            else
            {
                return GetClient();
            }
        }

        private HttpClient GetClient()
        {
            if (_rotateHttpClients)
            {
                lock (_lockObject)
                {

                    return _firstHttpClient ? _httpClient : _httpClient2;
                }
            }
            else
            {
                return _httpClient;
            }
        }

        private void CreateNewRotatedHttpClient()
        {
            var httpClient = CreateNewHttpClient();
            _httpClientLastCreatedAt = DateTime.UtcNow;
            
            if (_firstHttpClient)
            {
                lock (_lockObject)
                {
                    _firstHttpClient = false;
                    _httpClient2 = httpClient;
                }
            }
            else
            {
                lock (_lockObject)
                {
                    _firstHttpClient = true;
                    _httpClient = httpClient;
                }
            }
        }

        private HttpClient CreateNewHttpClient()
        {
            HttpClient httpClient = new HttpClient();
            
            if (_httpClientHandler != null)
            {
                httpClient = new HttpClient(_httpClientHandler);
            }
            else
            {
                var handler = GetDefaultHandler();
                if(handler != null)
                {
                    httpClient = new HttpClient(handler);
                }
            }

            InitialiseHttpClient(httpClient);
            return httpClient;
        }

        private void InitialiseHttpClient(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization = _authHeaderValue;
            httpClient.BaseAddress = _baseUrl;
        }
    }
}