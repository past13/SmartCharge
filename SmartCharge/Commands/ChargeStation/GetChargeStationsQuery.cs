using System.Collections.Generic;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class GetChargeStationsQuery: IRequest<Result<IEnumerable<ChargeStationEntity>>>
{
}