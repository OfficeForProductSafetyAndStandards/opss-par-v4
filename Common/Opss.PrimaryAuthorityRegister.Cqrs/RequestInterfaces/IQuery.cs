﻿using MediatR;

namespace Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;

/// <summary>
/// Defines a query
/// </summary>
/// <typeparam name="T">The return type of the query</typeparam>
public interface IQuery<out T> : IRequest<T>, IRequestBase
{ }
