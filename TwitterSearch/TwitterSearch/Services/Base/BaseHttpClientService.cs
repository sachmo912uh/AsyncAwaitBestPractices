﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Security.Cryptography;

using Newtonsoft.Json;

using Xamarin.Forms;

namespace TwitterSearch
{
    public abstract class BaseHttpClientService
    {
        #region Constant Fields
        const string _authorizationHeaderName = "Authorization";

        static readonly Lazy<JsonSerializer> _serializerHolder = new Lazy<JsonSerializer>();
        static readonly Lazy<HttpClient> _clientHolder = new Lazy<HttpClient>(() => CreateHttpClient(TimeSpan.FromSeconds(30)));
        #endregion

        #region Fields
        static int _networkIndicatorCount = 0;
        #endregion

        #region Properties
        protected static HttpClient Client => _clientHolder.Value;
        static JsonSerializer Serializer => _serializerHolder.Value;
        #endregion

        #region Methods
        protected static Task<HttpResponseMessage> GetResponseMessageFromAPI(string apiUrl) => GetResponseMessageFromAPI<object>(apiUrl);

        protected static async Task<HttpResponseMessage> GetResponseMessageFromAPI<TPayloadData>(string apiUrl, TPayloadData data = default)
        {

            var stringPayload = string.Empty;

            if (data?.Equals(default) == true)
                stringPayload = await Task.Run(() => JsonConvert.SerializeObject(data)).ConfigureAwait(false);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            AddOAutchAuthorizationHeader();

            try
            {
                UpdateActivityIndicatorStatus(true);

                return await Client.GetAsync(apiUrl);
            }
            catch (Exception)
            {
                return default;
            }
            finally
            {
                RemoveOAuthAuthorizationHeader();
                UpdateActivityIndicatorStatus(false);
            }
        }

        protected static Task<T> GetDataObjectFromAPI<T>(string apiUrl) => GetDataObjectFromAPI<T, object>(apiUrl);

        protected static async Task<TDataObject> GetDataObjectFromAPI<TDataObject, TPayloadData>(string apiUrl, TPayloadData data = default)
        {
            var stringPayload = string.Empty;

            if (data?.Equals(default) == true)
                stringPayload = await Task.Run(() => JsonConvert.SerializeObject(data)).ConfigureAwait(false);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            AddOAutchAuthorizationHeader();

            try
            {
                UpdateActivityIndicatorStatus(true);

                using (var stream = await Client.GetStreamAsync(apiUrl).ConfigureAwait(false))
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    if (json == null)
                        return default;

                    return await Task.Run(() => Serializer.Deserialize<TDataObject>(json)).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                return default;
            }
            finally
            {
                RemoveOAuthAuthorizationHeader();
                UpdateActivityIndicatorStatus(false);
            }
        }

        protected static async Task<HttpResponseMessage> PostObjectToAPI<T>(string apiUrl, T data)
        {
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(data)).ConfigureAwait(false);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            AddOAutchAuthorizationHeader();

            try
            {
                UpdateActivityIndicatorStatus(true);

                return await Client.PostAsync(apiUrl, httpContent).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                RemoveOAuthAuthorizationHeader();
                UpdateActivityIndicatorStatus(false);
            }
        }

        protected static async Task<HttpResponseMessage> PatchObjectToAPI<T>(string apiUrl, T data)
        {
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(data)).ConfigureAwait(false);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(apiUrl),
                Content = httpContent
            };

            AddOAutchAuthorizationHeader();

            try
            {
                UpdateActivityIndicatorStatus(true);

                return await Client.SendAsync(httpRequest).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                RemoveOAuthAuthorizationHeader();
                UpdateActivityIndicatorStatus(false);
            }
        }

        protected static async Task<HttpResponseMessage> DeleteObjectFromAPI(string apiUrl)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, new Uri(apiUrl));

            AddOAutchAuthorizationHeader();

            try
            {
                UpdateActivityIndicatorStatus(true);

                return await Client.SendAsync(httpRequest);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                RemoveOAuthAuthorizationHeader();
                UpdateActivityIndicatorStatus(false);
            }
        }

        static HttpClient CreateHttpClient(TimeSpan timeout)
        {
            HttpClient client;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                case Device.Android:
                    client = new HttpClient();
                    break;
                default:
                    client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
                    break;

            }
            client.Timeout = timeout;
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            return client;
        }

        static string GetOauthHeaderAuthorizationString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("OAuth ");
            stringBuilder.Append($"oauth_consumer_key=\"{TwitterConstants.ConsumerKey}\",");
            stringBuilder.Append($"oauth_token=\"{TwitterConstants.ConsumerToken}\",");
            stringBuilder.Append($"oauth_signature_method=\"HMAC - SHA1\",");
            stringBuilder.Append($"oauth_timestamp=\"{GenerateOAuthTimeStamp()}\",");
            stringBuilder.Append($"oauth_nonce=\"kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg\",");
            stringBuilder.Append($"oauth_version=\"1.0\",");
            stringBuilder.Append($"oauth_signature=\"{GenerateSha1Hash(TwitterConstants.AccessToken, TwitterConstants.AccessTokenSecret)}\"");

            return stringBuilder.ToString();

            #region Local Methods
            string GenerateOAuthTimeStamp()
            {
                TimeSpan timeSpan = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0));

                string timeStamp = timeSpan.TotalSeconds.ToString();
                timeStamp = timeStamp.Substring(0, timeStamp.IndexOf('.'));

                return timeStamp;
            }

            string GenerateSha1Hash(string key, string message)
            {
                var encoding = new ASCIIEncoding();

                byte[] keyBytes = encoding.GetBytes(key);
                byte[] messageBytes = encoding.GetBytes(message);

                string Sha1Result = string.Empty;

                using (var SHA1 = new HMACSHA1(keyBytes))
                {
                    var Hashed = SHA1.ComputeHash(messageBytes);
                    Sha1Result = Convert.ToBase64String(Hashed);
                }

                return Sha1Result;
            }
            #endregion
        }

        static void AddOAutchAuthorizationHeader()
        {
            if (Client.DefaultRequestHeaders.Contains(_authorizationHeaderName))
                Client.DefaultRequestHeaders.Add(_authorizationHeaderName, GetOauthHeaderAuthorizationString());
        }

        static void RemoveOAuthAuthorizationHeader() => Client.DefaultRequestHeaders.Remove(_authorizationHeaderName);

        static void UpdateActivityIndicatorStatus(bool isActivityInidicatorRunning)
        {
            if (isActivityInidicatorRunning)
            {
                Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.IsBusy = true);
                _networkIndicatorCount++;
            }
            else if (--_networkIndicatorCount <= 0)
            {
                Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.IsBusy = false);
                _networkIndicatorCount = 0;
            }
        }
        #endregion
    }
}
