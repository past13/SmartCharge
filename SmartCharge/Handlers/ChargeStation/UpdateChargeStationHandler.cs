using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands;
using SmartCharge.Domain.Entities;
using SmartCharge.Repository;

namespace SmartCharge.Handlers;

public class UpdateChargeStationHandler : IRequestHandler<UpdateChargeStationCommand, ChargeStationEntity>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;

    public UpdateChargeStationHandler(
        IGroupRepository groupRepository,
        IChargeStationRepository chargeStationRepository
        )
    {
        _groupRepository = groupRepository;
        _chargeStationRepository = chargeStationRepository;
    }
    
    public Task<ChargeStationEntity> Handle(UpdateChargeStationCommand request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}