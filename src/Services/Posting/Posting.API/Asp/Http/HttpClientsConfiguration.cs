using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Posting.API.Asp.Http
{
    public static class HttpClientsConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration, params string[] clients)
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

            foreach (var service in configuration.GetSection("Http").GetChildren()
                .Where(service => !string.IsNullOrWhiteSpace(service.GetValue<string>("BaseUri")))
                .Where(child => clients.Contains(child.Key)))
            {
                services.AddHttpClient(service.Key, client =>
                {
                    client.BaseAddress = new Uri(service.GetValue<string>("BaseUri"));
                    var headers = service.GetChildren().FirstOrDefault(x => x.Key == "Headers");

                    if (headers != null)
                    {
                        foreach (var header in headers.GetChildren())
                        {
                            client.DefaultRequestHeaders.Add(
                                header.GetValue<string>("Name"),
                                header.GetValue<string>("Value"));
                        }
                    }
                }).AddPolicyHandler(retryPolicy.WrapAsync(timeoutPolicy)); // timeoutPolicy is wrapped to time out each retry 
            }
        }
    }
}
