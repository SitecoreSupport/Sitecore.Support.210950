namespace Sitecore.Support.ContentSearch.Azure.Http
{
  using System;
  using System.Net.Http;
  using Sitecore.ContentSearch.Azure.Http;

  internal class HttpClientFactory : IHttpClientFactory
  {
    private IHttpClient client;

    private Object thisLock = new Object(); // Sitecore.Support.210950

    public TimeSpan ClientTimeout { get; set; }

    public IHttpClient Get(string baseAddress, string apiKey, ICloudSearchRetryPolicy retryPolicy)
    {
      var retryHandler = new RetryDelegatingHandler(new HttpClientHandler(), retryPolicy);

      return this.CreateClient(baseAddress, apiKey, retryHandler);
    }

    public IHttpClient Get(string baseAddress, string apiKey, IHttpMessageObserver observer, ICloudSearchRetryPolicy retryPolicy)
    {
      var retryHandler = new RetryDelegatingHandler(new HttpClientHandler(), retryPolicy);
      var observableHandler = new ObservableDelegatingHandler(retryHandler, observer);

      return this.CreateClient(baseAddress, apiKey, observableHandler);
    }

    private IHttpClient CreateClient(string baseAddress, string apiKey, DelegatingHandler retryHandler = null)
    {
      if (this.client == null)
      {
        #region Sitecore.Support.210950
        lock (thisLock)
        {
          if (this.client == null)
          {
            var client = retryHandler == null ? new HttpClientWrapper() : new HttpClientWrapper(retryHandler, this.ClientTimeout);
            client.AddDefaultHeader("api-key", apiKey);
            client.BaseAddress = new Uri(baseAddress);

            this.client = client;
          }
        }
        #endregion
      }
      return this.client;
    }
  }
}