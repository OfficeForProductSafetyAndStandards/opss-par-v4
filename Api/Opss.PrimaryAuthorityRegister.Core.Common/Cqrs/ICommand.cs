using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Opss.PrimaryAuthorityRegister.Core.Common.Cqrs;

/// <summary>
/// Defines a command that returns a value
/// </summary>
/// <typeparam name="T">T should be Guid</typeparam>
public interface ICommand<T> : IRequest<ActionResult<T>> { }

/// <summary>
/// Defines a command that doesn't return a value
/// </summary>
public interface ICommand : IRequest<ActionResult> { }
