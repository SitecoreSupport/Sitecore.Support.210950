namespace Sitecore.Support.ContentSearch.Azure.Http
{
  using System;
  using System.Collections.Generic;
  using System.Net.Http;
  using Sitecore.ContentSearch.Azure.Http;

  internal class HttpClientWrapper : IHttpClient
  {
    private readonly HttpClient httpClient;

    public HttpClientWrapper()
    {
      this.httpClient = new HttpClient();
    }

    public HttpClientWrapper(HttpMessageHandler retryHandler, TimeSpan httpClientTimeout)
    {
      this.httpClient = new HttpClient(retryHandler)
      {
        Timeout = httpClientTimeout == TimeSpan.Zero ? TimeSpan.FromMilliseconds(-1.0) : httpClientTimeout
      };
    }

    public HttpClientWrapper(HttpMessageHandler retryHandler) : this(retryHandler, TimeSpan.FromSeconds(100))
    {
    }

    public Uri BaseAddress
    {
      get { return this.httpClient.BaseAddress; }
      set { this.httpClient.BaseAddress = value; }
    }

    public void AddDefaultHeader(string key, params string[] values)
    {
      this.httpClient.DefaultRequestHeaders.Add(key, values);
    }

    public HttpResponseMessage Get(string requestUri, IEnumerable<KeyValuePair<string, string>> headers = null)
    {
      var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
      this.AddHeaders(httpRequestMessage, headers);

      return this.httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public HttpResponseMessage Post(string requestUri, HttpContent content, IEnumerable<KeyValuePair<string, string>> headers = null)
    {
      var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
      httpRequestMessage.Content = content;
      this.AddHeaders(httpRequestMessage, headers);

      return this.httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public HttpResponseMessage Put(string requestUri, HttpContent content, IEnumerable<KeyValuePair<string, string>> headers = null)
    {
      var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, requestUri);
      httpRequestMessage.Content = content;
      this.AddHeaders(httpRequestMessage, headers);

      return this.httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public HttpResponseMessage Delete(string requestUri, IEnumerable<KeyValuePair<string, string>> headers = null)
    {
      var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri);
      this.AddHeaders(httpRequestMessage, headers);

      return this.httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private void AddHeaders(HttpRequestMessage requestMessage, IEnumerable<KeyValuePair<string, string>> headers)
    {
      requestMessage.Headers.TryAddWithoutValidation("request-id", Guid.NewGuid().ToString());
      if (headers == null)
      {
        return;
      }

      foreach (var keyValuePair in headers)
      {
        requestMessage.Headers.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
      }
    }

    public void Dispose()
    {
      this.httpClient.Dispose();
    }
  }
}