using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CodeInvention.CuriosityLab.Application.Infrastructure
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : class
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        public PerformanceBehavior(ILogger<TRequest> logger)
        {
            _logger = logger;
            _timer = new Stopwatch();
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                _logger.LogWarning($"Long running process: {typeof(TRequest).Name} ({elapsedMilliseconds} milliseconds)");
            }

            return response;
        }
    }
}
