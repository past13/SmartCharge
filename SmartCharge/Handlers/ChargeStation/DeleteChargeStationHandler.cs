using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.ChargeStation;

public class DeleteChargeStationHandler : IRequestHandler<DeleteChargeStationCommand>, IRequest<Unit>
{
    private readonly IChargeStationRepository _chargeStationRepository;
    public DeleteChargeStationHandler(IChargeStationRepository chargeStationRepository)
    {
        _chargeStationRepository = chargeStationRepository;
    }
    
    public async Task<Unit> Handle(DeleteChargeStationCommand request, CancellationToken cancellationToken)
    {
        var chargeStation = await _chargeStationRepository.GetChargeStationById(request.Id);
        if (chargeStation == null)
        {
            throw new InvalidOperationException($"ChargeStation with ID {request.Id} not found.");
        }
        
        await _chargeStationRepository.DeleteChargeStationById(request.Id);
        
        return Unit.Value;
    }
}