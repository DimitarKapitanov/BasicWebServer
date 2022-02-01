using BasicWebServer.Server.Common;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using System;
using System.Collections.Generic;

namespace BasicWebServer.Server.Routing
{
    public class RoutingTable : IRoutingTable
    {
        private readonly Dictionary<Method, Dictionary<string, Func<Request, Response>>> routes;

        public RoutingTable() 
            => this.routes = new Dictionary<Method, Dictionary<string, Func<Request, Response>>>()
        {
            [Method.Get] = new Dictionary<string, Func<Request, Response>>(StringComparer.InvariantCultureIgnoreCase),

            [Method.Post] = new Dictionary<string, Func<Request, Response>>(StringComparer.InvariantCultureIgnoreCase),

            [Method.Put] = new Dictionary<string, Func<Request, Response>>(StringComparer.InvariantCultureIgnoreCase),

            [Method.Delete] = new Dictionary<string, Func<Request, Response>>(StringComparer.InvariantCultureIgnoreCase)
        };

        public IRoutingTable Map(
            Method method,
            string path,
            Func<Request, Response> responceFunction)
        {
            Guard.AgainstNull(path, nameof(path));
            Guard.AgainstNull(responceFunction, nameof(responceFunction));

            switch (method)
            {
                case Method.Get:
                    return MapGet(path, responceFunction);
                case Method.Post:
                    return MapPost(path, responceFunction);
                case Method.Put:
                case Method.Delete:
                default:
                    throw new ArgumentOutOfRangeException($"The method {method} is not supported!");
            }
        }

        private IRoutingTable MapGet(string path,
            Func<Request, Response> responceFunction)
        {
            routes[Method.Get][path] = responceFunction;

            return this;
        }

        private IRoutingTable MapPost(string path,
            Func<Request, Response> responceFunction)
        {
            routes[Method.Post][path] = responceFunction;

            return this;
        }

        public Response MatchRequest(Request request)
        {
            var requestMethod = request.Method;
            var requestUrl = request.Url;

            if (!this.routes.ContainsKey(requestMethod) || !this.routes[requestMethod].ContainsKey(requestUrl))
            {
                return new NotFoundResponse();
            }

            var responceFunction = this.routes[requestMethod][requestUrl];

            return responceFunction(request);
        }
    }
}
