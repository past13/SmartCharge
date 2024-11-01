using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests.ChargeStation;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Group;

public class CreateGroupCommand(string name, ChargeStationRequest chargeStation) : IRequest<Result<GroupEntity>>
{
    public string Name { get; set; } = name;
    public ChargeStationRequest ChargeStation { get; set; } = chargeStation;
}
