using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class myHttpClientProvider
    {
        protected readonly HttpClient _httpClient = new HttpClient();
        protected readonly LogFile log;

        public myHttpClientProvider(LogFile log, string url, string methodAuthorize = null, string token = null)
        {
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            _httpClient.BaseAddress = new Uri(url);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("IdiotoBrawser/100.500");

            if (methodAuthorize != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(methodAuthorize, token);

            this.log = log;
        }
        public bool MyRemoteCertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }

        #region POST
        public virtual async Task<(string Responce, bool status)> PostAsync<T>(string endpoint, T data, JsonSerializerSettings jsonSettings = null)
        {
            var jsonContent = SerializeToJson(data, jsonSettings);
            log.ToLog($"POST {endpoint}");

            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, httpContent);

            var charset = response.Content.Headers.ContentType?.CharSet;
            if (!string.IsNullOrEmpty(charset))
            {
                charset = charset.Trim('"');
            }

            var encoding = Encoding.GetEncoding(charset ?? "utf-8");

            var responseBytes = await response.Content.ReadAsByteArrayAsync();

            var content = encoding.GetString(responseBytes);

            await LogResponse(response);

            return (content, await LogResponse(response));
        }
        public async Task<(string Content, HttpStatusCode StatusCode)> StatusPostAsync<T>(string endpoint, T data, bool useFormUrlEncoded = false, JsonSerializerSettings jsonSettings = null)
        {
            log.ToLog($"POST {endpoint}");

            string contentType;
            HttpContent httpContent;

            if (useFormUrlEncoded && data is IDictionary<string, string> dictionaryData)
            {
                contentType = "application/x-www-form-urlencoded";
                httpContent = new FormUrlEncodedContent(dictionaryData);
            }
            else
            {
                contentType = "application/json";
                var jsonContent = SerializeToJson(data, jsonSettings);
                httpContent = new StringContent(jsonContent, Encoding.UTF8, contentType);
            }

            var response = await _httpClient.PostAsync(endpoint, httpContent);

            var charset = response.Content.Headers.ContentType?.CharSet;
            if (!string.IsNullOrEmpty(charset))
            {
                charset = charset.Trim('"');
            }

            var encoding = Encoding.GetEncoding(charset ?? "utf-8");

            var responseBytes = await response.Content.ReadAsByteArrayAsync();

            var content = encoding.GetString(responseBytes);

            await LogResponse(response);

            return (content, response.StatusCode);
        }
        public async Task<Y> PostAsync<T, Y>(string endpoint, T data)
        {
            (var responseBody, bool status) = await PostAsync<T>(endpoint, data);
            return JsonConvert.DeserializeObject<Y>(responseBody);
        }
        #endregion

        #region PUT
        public async Task<(string Response, bool Status)> PutAsync<T>(string endpoint, T data, JsonSerializerSettings jsonSettings = null)
        {
            var jsonContent = SerializeToJson(data, jsonSettings);
            log.ToLog($"PUT  {endpoint}");
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, httpContent);

            var charset = response.Content.Headers.ContentType?.CharSet;
            if (!string.IsNullOrEmpty(charset))
            {
                charset = charset.Trim('"');
            }

            var encoding = Encoding.GetEncoding(charset ?? "utf-8");


            var responseBytes = await response.Content.ReadAsByteArrayAsync();

            var content = encoding.GetString(responseBytes);

            return (content, await LogResponse(response));
        }
        #endregion

        #region GET
        public async Task<string> GetAsync(string endpoint)
        {
            log.ToLog($"GET  {endpoint}");
            var response = await _httpClient.GetAsync(endpoint);

            var charset = response.Content.Headers.ContentType?.CharSet;
            if (!string.IsNullOrEmpty(charset))
            {
                charset = charset.Trim('"');
            }

            var encoding = Encoding.GetEncoding(charset ?? "utf-8");

            var responseBytes = await response.Content.ReadAsByteArrayAsync();

            var content = encoding.GetString(responseBytes);

            await LogResponse(response);

            return content;
        }
        public async Task<T> GetAsync<T>(string endpoint)
        {
            return JsonConvert.DeserializeObject<T>(await GetAsync(endpoint));
        }
        public async Task<(string Content, HttpStatusCode StatusCode)> StatusGetAsync(string endpoint, List<Param> parameters = null)
        {
            if (parameters != null && parameters.Count > 0)
            {
                string queryString = "?" + string.Join("&", parameters.Select(p => $"{Uri.EscapeDataString(p.name)}={Uri.EscapeDataString(p.value)}"));
                endpoint += queryString;
            }

            log.ToLog($"GET  {endpoint}");

            var response = await _httpClient.GetAsync(endpoint);

            var charset = response.Content.Headers.ContentType?.CharSet?.Trim('"');
            var encoding = Encoding.GetEncoding(charset ?? "utf-8");

            var responseBytes = await response.Content.ReadAsByteArrayAsync();
            var content = encoding.GetString(responseBytes);

            await LogResponse(response);

            return (content, response.StatusCode);
        }
        #endregion

        #region PATCH
        public async Task<(string Content, HttpStatusCode StatusCode)> StatusPatchAsync<T>(string endpoint, T data, JsonSerializerSettings jsonSettings = null)
        {
            log.ToLog($"PATCH {endpoint}");

            var jsonContent = SerializeToJson(data, jsonSettings);

            string contentType = "application/json";
            HttpContent httpContent = new StringContent(jsonContent, Encoding.UTF8, contentType);

            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"), // Створюємо PATCH метод вручну
                RequestUri = new Uri(endpoint, UriKind.RelativeOrAbsolute),
                Content = httpContent
            };

            var response = await _httpClient.SendAsync(request);

            var charset = response.Content.Headers.ContentType?.CharSet;
            if (!string.IsNullOrEmpty(charset))
            {
                charset = charset.Trim('"');
            }

            var encoding = Encoding.GetEncoding(charset ?? "utf-8");

            var responseBytes = await response.Content.ReadAsByteArrayAsync();

            var content = encoding.GetString(responseBytes);

            await LogResponse(response);

            return (content, response.StatusCode);
        }
        #endregion

        #region DEL

        public async Task<(string Content, HttpStatusCode StatusCode)> StatusDelAsync(string endpoint)
        {
            log.ToLog($"DEL {endpoint}");

            var response = await _httpClient.DeleteAsync(endpoint);

            var charset = response.Content.Headers.ContentType?.CharSet;
            if (!string.IsNullOrEmpty(charset))
            {
                charset = charset.Trim('"');
            }

            var encoding = Encoding.GetEncoding(charset ?? "utf-8");

            var responseBytes = await response.Content.ReadAsByteArrayAsync();

            var content = encoding.GetString(responseBytes);

            await LogResponse(response);

            return (content, response.StatusCode);
        }

        public async Task<(string Content, HttpStatusCode StatusCode)> StatusDelAsync<T>(string endpoint, T data, JsonSerializerSettings jsonSettings = null)
        {
            log.ToLog($"DEL {endpoint}");

            string contentType = "application/json";
            var jsonContent = SerializeToJson(data, jsonSettings);
            HttpContent httpContent = new StringContent(jsonContent, Encoding.UTF8, contentType);

            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = httpContent
            };

            var response = await _httpClient.SendAsync(request);

            var charset = response.Content.Headers.ContentType?.CharSet;
            if (!string.IsNullOrEmpty(charset))
            {
                charset = charset.Trim('"');
            }

            var encoding = Encoding.GetEncoding(charset ?? "utf-8");

            var responseBytes = await response.Content.ReadAsByteArrayAsync();

            var content = encoding.GetString(responseBytes);

            await LogResponse(response);

            return (content, response.StatusCode);
        }
        #endregion
        public async Task<bool> DownloadFileAsync(string remoteFile, string localFile)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(remoteFile))
                {
                    response.EnsureSuccessStatusCode();
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = File.Create(localFile))
                    {
                        await stream.CopyToAsync(fileStream);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ToLog($"Error DownloadFileAsync: {ex.Message}");
            }
            return false;
        }
        public string SerializeToJson<T>(T request, JsonSerializerSettings jsonSettings = null)
        {
            if (jsonSettings == null)
                jsonSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                };

            string jsonString = JsonConvert.SerializeObject(request, jsonSettings);
            return jsonString;
        }
        public async Task<bool> LogResponse(HttpResponseMessage response)
        {
            var charset = response.Content.Headers.ContentType?.CharSet;
            if (!string.IsNullOrEmpty(charset))
            {
                charset = charset.Trim('"');
            }

            var encoding = Encoding.GetEncoding(charset ?? "utf-8");

            var responseBytes = await response.Content.ReadAsByteArrayAsync();

            var responseBody = encoding.GetString(responseBytes);

            string formattedResponseBody;

            try
            {
                var parsedJson = JToken.Parse(responseBody);
                formattedResponseBody = parsedJson.ToString(Formatting.Indented);
            }
            catch (JsonReaderException)
            {
                // If responseBody is not a valid JSON, keep it as is
                formattedResponseBody = responseBody;
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.Created:
                case HttpStatusCode.NoContent:
                    return true;

                case HttpStatusCode.BadRequest:
                    Error(formattedResponseBody);
                    log.ToLog($"Некоректний запит: {response.StatusCode} - {SaveBodyIfTooLong(formattedResponseBody)}");
                    return false;

                case HttpStatusCode.Unauthorized:
                    Error(formattedResponseBody);
                    log.ToLog($"Неавторизований: {response.StatusCode} - {SaveBodyIfTooLong(formattedResponseBody)}");
                    return false;

                case HttpStatusCode.InternalServerError:
                    Error(formattedResponseBody);
                    log.ToLog($"Внутрішня помилка сервера: {response.StatusCode} - {SaveBodyIfTooLong(formattedResponseBody)}");
                    return false;

                default:
                    Error(formattedResponseBody);
                    log.ToLog($"Помилка: {response.StatusCode} - {SaveBodyIfTooLong(formattedResponseBody)}");
                    return false;
            }

        }
        string SaveBodyIfTooLong(string body)
        {
            if (body.Length > 100)
            {
                log.ToLog($"[Response body is too long. Saved to file]");
            }

            return body;
        }
        private void Error(string body)
        {
            string textError = "";

            if (body.IndexOf("fortinet", StringComparison.OrdinalIgnoreCase) >= 0 || body.IndexOf("FortiGuard", StringComparison.OrdinalIgnoreCase) >= 0)
                textError = "FORTIGUARD";
            else
                textError = body.Length > 200 ? body.Substring(0, 200) : body;
        }
        public class Param
        {
            public string name;
            public string value;
        }
    }
}
