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

public class GetChargeStationsHandler(
    IUnitOfWork unitOfWork,
    IChargeStationRepository chargeStationRepository)
    : IRequestHandler<GetChargeStationsQuery, Result<IEnumerable<ChargeStationEntity>>>
{
    public async Task<Result<IEnumerable<ChargeStationEntity>>> Handle(GetChargeStationsQuery query, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var chargeStations = await chargeStationRepository.GetChargeStations();
            
            await unitOfWork.CommitAsync();
            
            return Result<IEnumerable<ChargeStationEntity>>.Success(chargeStations);
        }
        catch (ArgumentException ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<IEnumerable<ChargeStationEntity>>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<IEnumerable<ChargeStationEntity>>.Failure(ex.Message);
        }
    }
}