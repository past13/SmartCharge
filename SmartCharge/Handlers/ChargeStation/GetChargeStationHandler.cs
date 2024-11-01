using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.ChargeStation;

public class GetChargeStationHandler : IRequestHandler<GetChargeStationByIdQuery, Result<ChargeStationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IMapper _mapper;
    
    public GetChargeStationHandler(
        IUnitOfWork unitOfWork,
        IChargeStationRepository chargeStationRepository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _chargeStationRepository = chargeStationRepository;
        _mapper = mapper;
    }
    
    public async Task<Result<ChargeStationDto>> Handle(GetChargeStationByIdQuery query, CancellationToken cancellationToken)
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
            
            return Result<ChargeStationDto>.Success(_mapper.Map<ChargeStationDto>(chargeStation));
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ChargeStationDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ChargeStationDto>.Failure(ex.Message);
        }
    }
}