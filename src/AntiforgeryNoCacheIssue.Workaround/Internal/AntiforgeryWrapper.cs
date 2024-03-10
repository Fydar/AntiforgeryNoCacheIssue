using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace AntiforgeryNoCacheIssue.Workaround.Internal;

internal class AntiforgeryWrapper(
    [FromKeyedServices("DefaultAntiforgery")] IAntiforgery defaultAntiforgery) : IAntiforgery
{
    public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
    {
        var result = defaultAntiforgery.GetAndStoreTokens(httpContext);

        if (!httpContext.Response.HasStarted)
        {
            SetDoNotCacheHeaders(httpContext);
        }

        return result;
    }

    public AntiforgeryTokenSet GetTokens(HttpContext httpContext)
    {
        return defaultAntiforgery.GetTokens(httpContext);
    }

    public Task<bool> IsRequestValidAsync(HttpContext httpContext)
    {
        return defaultAntiforgery.IsRequestValidAsync(httpContext);
    }

    public void SetCookieTokenAndHeader(HttpContext httpContext)
    {
        defaultAntiforgery.SetCookieTokenAndHeader(httpContext);

        if (!httpContext.Response.HasStarted)
        {
            SetDoNotCacheHeaders(httpContext);
        }
    }

    public Task ValidateRequestAsync(HttpContext httpContext)
    {
        return defaultAntiforgery.ValidateRequestAsync(httpContext);
    }

    private static void SetDoNotCacheHeaders(HttpContext httpContext)
    {
        httpContext.Response.Headers[HeaderNames.CacheControl] = "no-cache";
    }
}
