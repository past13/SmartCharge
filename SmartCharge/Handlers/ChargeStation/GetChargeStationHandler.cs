using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.ChargeStation;

public class GetChargeStationHandler(
    IUnitOfWork unitOfWork,
    IChargeStationRepository chargeStationRepository)
    : IRequestHandler<GetChargeStationByIdQuery, Result<ChargeStationEntity>>
{
    public async Task<Result<ChargeStationEntity>> Handle(GetChargeStationByIdQuery query, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var chargeStation = await chargeStationRepository.GetChargeStationById(query.Id);
            if (chargeStation is null)
            {
                throw new ArgumentException($"A ChargeStation with Id {query.Id} does not exist.");
            }
                
            chargeStation.IsValidForChange();
            
            await unitOfWork.CommitAsync();
            
            return Result<ChargeStationEntity>.Success(chargeStation);
        }
        catch (ArgumentException ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<ChargeStationEntity>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<ChargeStationEntity>.Failure(ex.Message);
        }
    }
}