using Ardalis.Result;
using MediatR;

namespace AuthenticationService.Features.CheckExistence;

public sealed record CheckExistenceRequest(Guid UserId) : IRequest<Result>;
