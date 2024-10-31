using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.ChargeStation;

public class DeleteChargeStationHandler : IRequestHandler<DeleteChargeStationCommand, Result<ChargeStationEntity>>
{
    private readonly IChargeStationRepository _chargeStationRepository;
    public DeleteChargeStationHandler(IChargeStationRepository chargeStationRepository)
    {
        _chargeStationRepository = chargeStationRepository;
    }
    
    public async Task<Result<ChargeStationEntity>> Handle(DeleteChargeStationCommand command, CancellationToken cancellationToken)
    {
        var result = await _chargeStationRepository.DeleteChargeStationById(command.Id);
        return result;
    }
}