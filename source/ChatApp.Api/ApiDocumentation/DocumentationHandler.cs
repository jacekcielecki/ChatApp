using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ChatApp.Api.ApiDocumentation
{
    public static class ApiDocs
    {
        /// <summary>
        /// Adds endpoint for Api documentation.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> for HttpsRedirection.</returns>
        public static IApplicationBuilder UseApiDocumentation(this WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.MapGet("/Api/docs", (IActionDescriptorCollectionProvider provider) =>
            {
                var minimalApiEndpoints = app.Services.GetServices<EndpointDataSource>().SelectMany(
                    eds => eds.Endpoints.Select(e => new Endpoint
                    {
                        Name = e.DisplayName,
                        Method = e.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.FirstOrDefault()
                    })).ToArray();

                var controllerEndpoints = provider.ActionDescriptors.Items.Where(
                    ad => ad.AttributeRouteInfo != null).Select(
                    ad => new Endpoint
                    {
                        Name = ad.AttributeRouteInfo?.Name,
                        Method = ad.AttributeRouteInfo?.Template
                    }).ToArray();

                Endpoint[] endpoints = [.. minimalApiEndpoints, .. controllerEndpoints];

                var html = File.ReadAllText("./ApiDocumentation/index.html");
                var content = "";

                foreach (var endpoint in endpoints)
                {
                    content += $"<li> Endpoint: {endpoint.Name} {endpoint.Method} </li>";
                }

                html = html.Replace("{{content}}", content);

                return Results.Content(html, "text/html");
            });

            return app;
        }
    }
}
