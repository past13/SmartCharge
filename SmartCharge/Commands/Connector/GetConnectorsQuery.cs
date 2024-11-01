using System.Collections.Generic;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Connector;

public class GetConnectorsQuery : IRequest<Result<IEnumerable<ConnectorEntity>>>
{
}