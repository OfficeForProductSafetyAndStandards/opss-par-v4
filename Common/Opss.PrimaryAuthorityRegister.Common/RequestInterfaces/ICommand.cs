using MediatR;

namespace Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

/// <summary>
/// Defines a command that returns a value
/// </summary>
/// <typeparam name="T">T should be Guid</typeparam>
public interface ICommand<out T> : IRequest<T>, ICommandBase { }

/// <summary>
/// Defines a command that doesn't return a value
/// </summary>
public interface ICommand : IRequest, ICommandBase { }
