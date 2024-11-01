using System;
using MediatR;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class GetChargeStationByIdQuery: IRequest<Result<ChargeStationDto>>
{
    public Guid Id { get; }

    public GetChargeStationByIdQuery(Guid id)
    {
        Id = id;
    }
}