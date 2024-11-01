﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.ChargeStation;

public class GetChargeStationHandler : IRequestHandler<GetChargeStationByIdQuery, Result<ChargeStationEntity>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChargeStationRepository _chargeStationRepository;
    
    public GetChargeStationHandler(
        IUnitOfWork unitOfWork,
        IChargeStationRepository chargeStationRepository)
    {
        _unitOfWork = unitOfWork;
        _chargeStationRepository = chargeStationRepository;
    }
    
    public async Task<Result<ChargeStationEntity>> Handle(GetChargeStationByIdQuery query, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var chargeStation = await _chargeStationRepository.GetChargeStationById(query.Id);
            if (chargeStation is null)
            {
                throw new ArgumentException($"A ChargeStation with Id {query.Id} does not exist.");
            }
                
            chargeStation.IsValidForChange();
            
            await _unitOfWork.CommitAsync();
            
            return Result<ChargeStationEntity>.Success(chargeStation);
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ChargeStationEntity>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ChargeStationEntity>.Failure(ex.Message);
        }
    }
}