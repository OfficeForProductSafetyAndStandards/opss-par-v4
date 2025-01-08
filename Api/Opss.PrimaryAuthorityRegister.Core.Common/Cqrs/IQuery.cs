using MediatR;

namespace Opss.PrimaryAuthorityRegister.Core.Common.Cqrs;

/// <summary>
/// Defines a query
/// </summary>
/// <typeparam name="T">The return type of the query</typeparam>
public interface IQuery<T> : IRequest<T>
{ }
