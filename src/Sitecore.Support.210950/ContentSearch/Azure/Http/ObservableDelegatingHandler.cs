namespace Sitecore.Support.ContentSearch.Azure.Http
{
  using System;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using Sitecore.ContentSearch.Azure.Http;

  internal class ObservableDelegatingHandler : DelegatingHandler
  {
    private readonly IHttpMessageObserver observer;

    public ObservableDelegatingHandler(HttpMessageHandler innerHandler, IHttpMessageObserver observer) : base(innerHandler)
    {
      this.observer = observer;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      HttpResponseMessage result = null;
      try
      {
        result = await base.SendAsync(request, cancellationToken);

        this.observer.HandleMessage(result);
      }
      catch (Exception exception)
      {
        this.observer.HandleException(exception);
        throw;
      }

      return result;
    }
  }
}