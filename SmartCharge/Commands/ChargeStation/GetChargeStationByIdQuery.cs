using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class GetChargeStationByIdQuery(Guid id) : IRequest<Result<ChargeStationEntity>>
{
    public Guid Id { get; } = id;
}