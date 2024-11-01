using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class GetChargeStationByIdQuery: IRequest<Result<ChargeStationEntity>>
{
    public Guid Id { get; }

    public GetChargeStationByIdQuery(Guid id)
    {
        Id = id;
    }
}