using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace mlidynamics.iot.azure
{
    public class HttpClientHelper
    {
        private const string ApiVersion = "&api-version=2015-03";
        private readonly string _address;
        private readonly HttpClient _httpClient;

        public HttpClientHelper(string address, string token)
        {
            _httpClient = new HttpClient();
            _address = address;
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);
            _httpClient.DefaultRequestHeaders.Add("ContentType", "application/atom+xml;type=entry;charset=utf-8");
        }

        public async Task<byte[]> GetEntity()
        {
            HttpResponseMessage response = null;
            try
            {
                response = await _httpClient.GetAsync(_address + "?timeout=60" + ApiVersion);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("GetEntity failed: " + ex.Message);
            }
            var entityDescription = await response.Content.ReadAsByteArrayAsync();
            return entityDescription;
        }

        // Create event hub. Returns 0 on success, 1 if event hub exists, -1 if event hub could not be created.
        public async Task<int> CreateEntity(byte[] entityDescription)
        {
            HttpResponseMessage response = null;
            try
            {
                response =
                    await
                        _httpClient.PutAsync(_address + "?timeout=60" + ApiVersion,
                            new ByteArrayContent(entityDescription));
                response.EnsureSuccessStatusCode();
                return 0;
            }
            catch (HttpRequestException ex)
            {
                if (response != null)
                {
                    Console.WriteLine("HTTP Status Code: " + (int) response.StatusCode);
                    if ((int) response.StatusCode == 409)
                    {
                        Console.WriteLine("Entity " + _address + " already exists.");
                        return 1;
                    }
                }
                Console.WriteLine("CreateEntity failed: " + ex.Message);
                return -1;
            }
        }

        public async Task UpdateEntity(byte[] entityDescription)
        {
            _httpClient.DefaultRequestHeaders.Add("If-Match", "*");
            HttpResponseMessage response = null;
            try
            {
                response =
                    await
                        _httpClient.PutAsync(_address + "?timeout=60" + ApiVersion,
                            new ByteArrayContent(entityDescription));
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (response != null)
                {
                    Console.WriteLine("HTTP Status Code: " + (int) response.StatusCode);
                    if ((int) response.StatusCode == 409)
                    {
                        Console.WriteLine("Entity " + _address + " already exists.");
                        return;
                    }
                }
                Console.WriteLine("UpdateEntity failed: " + ex.Message);
            }
        }

        public async Task DeleteEntity()
        {
            HttpResponseMessage response = null;
            try
            {
                response = await _httpClient.DeleteAsync(_address + "?timeout=60" + ApiVersion);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("DeleteEntity failed: " + ex.Message);
            }
        }

        public async Task SendMessage(string messageBody, NameValueCollection customProperties = null)
        {
            HttpContent postContent = new ByteArrayContent(Encoding.UTF8.GetBytes(messageBody));

            // Add custom properties.
            if (customProperties != null)
            {
                foreach (string key in customProperties)
                {
                    postContent.Headers.Add(key, customProperties.GetValues(key));
                }
            }

            // Send message.
            HttpResponseMessage response = null;
            try
            {
                response = await _httpClient.PostAsync(_address + "/messages" + "?timeout=60" + ApiVersion, postContent);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("SendMessage failed: " + ex.Message);
            }
        }
    }
}