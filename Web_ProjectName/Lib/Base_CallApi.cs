using Newtonsoft.Json;
using System.Net.Http.Headers;
using System;

namespace Web_ProjectName.Lib
{
    public interface IBase_CallApi
    {
        string _factoryName { get; set; }
        Task<ResponseData<T>> GetResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, string accessToken = "");
        Task<ResponseData<T>> GetDictHeaderResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, Dictionary<string, dynamic> dictHeads = default(Dictionary<string, dynamic>));
        Task<ResponseData<T>> PostResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, string accessToken = "");
        Task<ResponseData<T>> PostDictHeaderResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, Dictionary<string, dynamic> dictHeads = default(Dictionary<string, dynamic>));
        Task<ResponseData<T>> PutResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, string accessToken = "");
        Task<ResponseData<T>> PutDictHeaderResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, Dictionary<string, dynamic> dictHeads = default(Dictionary<string, dynamic>));
        Task<ResponseData<T>> DeleteResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, string accessToken = "");
        Task<ResponseData<T>> DeleteDictHeaderResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, Dictionary<string, dynamic> dictHeads = default(Dictionary<string, dynamic>));
        Task<ResponseData<T>> PostResponseDataAsync<T>(string url, MultipartFormDataContent formData, string accessToken = "");
        Task<ResponseData<T>> PostResponseDataAsync<T>(string url, MultipartFormDataContent formData, Dictionary<string, dynamic> dictHeads = default(Dictionary<string, dynamic>));
        Task<ResponseData<T>> PostResponseDataAsync<T>(string url, FormUrlEncodedContent xwwwFormUrlEndcoded, string accessToken = "");
        Task<ResponseData<T>> PutResponseDataAsync<T>(string url, MultipartFormDataContent formData, string accessToken = "");
        Task<ResponseData<T>> PutResponseDataAsync<T>(string url, FormUrlEncodedContent xwwwFormUrlEndcoded, string accessToken = "");
    }
    public class Base_CallApi : IBase_CallApi
    {
        public string _factoryName { get; set; }
        private readonly IHttpClientFactory _factory;
        public Base_CallApi(IHttpClientFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<ResponseData<T>> GetResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, string accessToken = "")
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                HttpClient client = _factory.CreateClient(_factoryName);
                if (!string.IsNullOrEmpty(accessToken))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                int i = 0;
                string param = "?";
                if (dictPars != null)
                    foreach (KeyValuePair<string, dynamic> item in dictPars)
                    {
                        param += (i == 0 ? "" : "&") + string.Format("{0}={1}", item.Key, item.Value == null ? "" : item.Value.ToString());
                        i++;
                    }
                var response = await client.GetAsync(url + param);
                response.EnsureSuccessStatusCode();
                var jsonres = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                return res;
            }
            catch (HttpRequestException ex)
            {
                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }
        }
        public async Task<ResponseData<T>> GetDictHeaderResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, Dictionary<string, dynamic> dictHeads = default(Dictionary<string, dynamic>))
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                HttpClient client = _factory.CreateClient(_factoryName);
                if (dictHeads != null)
                    foreach (KeyValuePair<string, dynamic> item in dictHeads)
                        client.DefaultRequestHeaders.Add(item.Key, item.Value == null ? "" : item.Value.ToString());
                int i = 0;
                string param = "?";
                if (dictPars != null)
                    foreach (KeyValuePair<string, dynamic> item in dictPars)
                    {
                        param += (i == 0 ? "" : "&") + string.Format("{0}={1}", item.Key, item.Value == null ? "" : item.Value.ToString());
                        i++;
                    }
                var response = await client.GetAsync(url + param);
                response.EnsureSuccessStatusCode();
                var jsonres = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                return res;
            }
            catch (HttpRequestException ex)
            {
                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }
        }
        public async Task<ResponseData<T>> PostResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, string accessToken = "")
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                HttpClient client = _factory.CreateClient(_factoryName);
                if (!string.IsNullOrEmpty(accessToken))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                MultipartFormDataContent formData = new MultipartFormDataContent();
                if (dictPars != null)
                    foreach (KeyValuePair<string, dynamic> item in dictPars)
                        formData.Add(new StringContent(item.Value == null ? "" : item.Value.ToString()), item.Key);

                var response = await client.PostAsync(url, formData);
                response.EnsureSuccessStatusCode();
                var jsonres = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                return res;
            }
            catch (HttpRequestException ex)
            {
                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }
        }
        public async Task<ResponseData<T>> PostDictHeaderResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, Dictionary<string, dynamic> dictHeads = default(Dictionary<string, dynamic>))
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                HttpClient client = _factory.CreateClient(_factoryName);
                if (dictHeads != null)
                    foreach (KeyValuePair<string, dynamic> item in dictHeads)
                        client.DefaultRequestHeaders.Add(item.Key, item.Value == null ? "" : item.Value.ToString());
                MultipartFormDataContent formData = new MultipartFormDataContent();
                if (dictPars != null)
                    foreach (KeyValuePair<string, dynamic> item in dictPars)
                        formData.Add(new StringContent(item.Value == null ? "" : item.Value.ToString()), item.Key);

                var response = await client.PostAsync(url, formData);
                response.EnsureSuccessStatusCode();
                var jsonres = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                return res;
            }
            catch (HttpRequestException ex)
            {
                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }
        }
        public async Task<ResponseData<T>> PutResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, string accessToken = "")
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - URL: {url}");
                System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Factory Name: {_factoryName}");
                System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Access Token: {accessToken ?? "NULL"}");

                HttpClient client = _factory.CreateClient(_factoryName);
                System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - HttpClient created successfully");

                if (!string.IsNullOrEmpty(accessToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Authorization header added");
                }

                if (url.Contains("News/Update"))
                {
                    MultipartFormDataContent formData = new MultipartFormDataContent();
                    if (dictPars != null)
                        foreach (KeyValuePair<string, dynamic> item in dictPars)
                        {
                            formData.Add(new StringContent(item.Value == null ? "" : item.Value.ToString()), item.Key);
                        }

                    string fullUrl = url;
                    System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Full URL: {fullUrl}");
                    System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Base Address: {client.BaseAddress}");
                    System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Parameters: {System.Text.Json.JsonSerializer.Serialize(dictPars)}");
                    System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Timeout: {client.Timeout}");

                    var response = await client.PutAsync(fullUrl, formData);
                    System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Response Status: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Response Headers: {System.Text.Json.JsonSerializer.Serialize(response.Headers)}");

                    response.EnsureSuccessStatusCode();
                    var jsonres = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Response Content: {jsonres}");

                    res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                    return res;
                }
                else
                {
                    MultipartFormDataContent formData = new MultipartFormDataContent();
                    if (dictPars != null)
                        foreach (KeyValuePair<string, dynamic> item in dictPars)
                            formData.Add(new StringContent(item.Value == null ? "" : item.Value.ToString()), item.Key);

                    var response = await client.PutAsync(url, formData);
                    response.EnsureSuccessStatusCode();
                    var jsonres = await response.Content.ReadAsStringAsync();
                    res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                    return res;
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - HttpRequestException: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - HttpRequestException StatusCode: {ex.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - HttpRequestException StackTrace: {ex.StackTrace}");

                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - TaskCanceledException: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - TaskCanceledException StackTrace: {ex.StackTrace}");

                res.result = -1;
                res.error = new error() { code = -1, message = "Lỗi timeout kết nối đến máy chủ" };
                return res;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Exception Type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Base_CallApi PutResponseDataAsync - Exception StackTrace: {ex.StackTrace}");

                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }
        }
        public async Task<ResponseData<T>> PutDictHeaderResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, Dictionary<string, dynamic> dictHeads = default(Dictionary<string, dynamic>))
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                HttpClient client = _factory.CreateClient(_factoryName);
                if (dictHeads != null)
                    foreach (KeyValuePair<string, dynamic> item in dictHeads)
                        client.DefaultRequestHeaders.Add(item.Key, item.Value == null ? "" : item.Value.ToString());
                MultipartFormDataContent formData = new MultipartFormDataContent();
                if (dictPars != null)
                    foreach (KeyValuePair<string, dynamic> item in dictPars)
                        formData.Add(new StringContent(item.Value == null ? "" : item.Value.ToString()), item.Key);

                var response = await client.PutAsync(url, formData);
                response.EnsureSuccessStatusCode();
                var jsonres = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                return res;
            }
            catch (HttpRequestException ex)
            {
                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }
        }
        public async Task<ResponseData<T>> DeleteResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, string accessToken = "")
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                HttpClient client = _factory.CreateClient(_factoryName);
                if (!string.IsNullOrEmpty(accessToken))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                int i = 0;
                string param = "?";
                if (dictPars != null)
                    foreach (KeyValuePair<string, dynamic> item in dictPars)
                    {
                        param += (i == 0 ? "" : "&") + string.Format("{0}={1}", item.Key, item.Value == null ? "" : item.Value.ToString());
                        i++;
                    }
                var response = await client.DeleteAsync(url + param);
                response.EnsureSuccessStatusCode();
                var jsonres = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                return res;
            }
            catch (HttpRequestException ex)
            {
                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }
        }
        public async Task<ResponseData<T>> DeleteDictHeaderResponseDataAsync<T>(string url, Dictionary<string, dynamic> dictPars, Dictionary<string, dynamic> dictHeads = default(Dictionary<string, dynamic>))
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                HttpClient client = _factory.CreateClient(_factoryName);
                if (dictHeads != null)
                    foreach (KeyValuePair<string, dynamic> item in dictHeads)
                        client.DefaultRequestHeaders.Add(item.Key, item.Value == null ? "" : item.Value.ToString());
                int i = 0;
                string param = "?";
                if (dictPars != null)
                    foreach (KeyValuePair<string, dynamic> item in dictPars)
                    {
                        param += (i == 0 ? "" : "&") + string.Format("{0}={1}", item.Key, item.Value == null ? "" : item.Value.ToString());
                        i++;
                    }
                var response = await client.DeleteAsync(url + param);
                response.EnsureSuccessStatusCode();
                var jsonres = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                return res;
            }
            catch (HttpRequestException ex)
            {
                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }
        }
        public async Task<ResponseData<T>> PostResponseDataAsync<T>(string url, MultipartFormDataContent formData, Dictionary<string, dynamic> dictHeads = default(Dictionary<string, dynamic>))
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                HttpClient client = _factory.CreateClient(_factoryName);
                if (dictHeads != null)
                    foreach (KeyValuePair<string, dynamic> item in dictHeads)
                        client.DefaultRequestHeaders.Add(item.Key, item.Value == null ? "" : item.Value.ToString());
                var response = await client.PostAsync(url, formData);
                response.EnsureSuccessStatusCode();
                var jsonres = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                return res;
            }
            catch (HttpRequestException ex)
            {
                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }

        }
        public async Task<ResponseData<T>> PostResponseDataAsync<T>(string url, MultipartFormDataContent formData, string accessToken = "")
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                HttpClient client = _factory.CreateClient(_factoryName);
                if (!string.IsNullOrEmpty(accessToken))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.PostAsync(url, formData);
                response.EnsureSuccessStatusCode();
                var jsonres = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                return res;
            }
            catch (HttpRequestException ex)
            {
                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }

        }
        public async Task<ResponseData<T>> PostResponseDataAsync<T>(string url, FormUrlEncodedContent xwwwFormUrlEndcoded, string accessToken = "")
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                HttpClient client = _factory.CreateClient(_factoryName);
                if (!string.IsNullOrEmpty(accessToken))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.PostAsync(url, xwwwFormUrlEndcoded);
                response.EnsureSuccessStatusCode();
                var jsonres = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                return res;
            }
            catch (HttpRequestException ex)
            {
                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }

        }
        public async Task<ResponseData<T>> PutResponseDataAsync<T>(string url, MultipartFormDataContent formData, string accessToken = "")
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                HttpClient client = _factory.CreateClient(_factoryName);
                if (!string.IsNullOrEmpty(accessToken))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.PutAsync(url, formData);
                response.EnsureSuccessStatusCode();
                var jsonres = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                return res;
            }
            catch (HttpRequestException ex)
            {
                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }

        }
        public async Task<ResponseData<T>> PutResponseDataAsync<T>(string url, FormUrlEncodedContent xwwwFormUrlEndcoded, string accessToken = "")
        {
            ResponseData<T> res = new ResponseData<T>();
            try
            {
                HttpClient client = _factory.CreateClient(_factoryName);
                if (!string.IsNullOrEmpty(accessToken))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.PutAsync(url, xwwwFormUrlEndcoded);
                response.EnsureSuccessStatusCode();
                var jsonres = await response.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ResponseData<T>>(jsonres);
                return res;
            }
            catch (HttpRequestException ex)
            {
                res.result = -1;
                res.error = new error() { code = ex.StatusCode.HasValue ? (int)ex.StatusCode : -1, message = ex.Message };
                return res;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error = new error() { code = -1, message = ex.Message };
                return res;
            }

        }
    }
}
