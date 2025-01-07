using MediatR;

namespace Opss.PrimaryAuthorityRegister.Core.Api.Handlers
{
    /// <summary>
    /// A Base for IRequestHandler providing additional functionality common to all handlers
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public abstract class BaseRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<BaseRequestHandler<TRequest, TResponse>> _logger;

        /// <summary>
        /// Constructor for Dependency Injection
        /// </summary>
        /// <param name="logger">An ILogger</param>
        /// <exception cref="ArgumentNullException">Thrown if ILogger is null</exception>
        protected BaseRequestHandler(ILogger<BaseRequestHandler<TRequest, TResponse>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from the request</returns>
        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
