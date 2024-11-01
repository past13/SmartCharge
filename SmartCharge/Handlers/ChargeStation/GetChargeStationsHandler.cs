using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.ChargeStation;

public class GetChargeStationsHandler : IRequestHandler<GetChargeStationsQuery, Result<IEnumerable<ChargeStationEntity>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChargeStationRepository _chargeStationRepository;
    
    public GetChargeStationsHandler(
        IUnitOfWork unitOfWork,
        IChargeStationRepository chargeStationRepository)
    {
        _unitOfWork = unitOfWork;
        _chargeStationRepository = chargeStationRepository;
    }
    
    public async Task<Result<IEnumerable<ChargeStationEntity>>> Handle(GetChargeStationsQuery query, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var chargeStations = await _chargeStationRepository.GetChargeStations();
            
            await _unitOfWork.CommitAsync();
            
            return Result<IEnumerable<ChargeStationEntity>>.Success(chargeStations);
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<IEnumerable<ChargeStationEntity>>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<IEnumerable<ChargeStationEntity>>.Failure(ex.Message);
        }
    }
}