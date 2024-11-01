using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.ChargeStation;

public class CreateChargeStationHandler : IRequestHandler<CreateChargeStationCommand, Result<ChargeStationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    
    public CreateChargeStationHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IChargeStationRepository chargeStationRepository,
        IGroupRepository groupRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _chargeStationRepository = chargeStationRepository;
        _groupRepository = groupRepository;
    }
    
    public async Task<Result<ChargeStationDto>> Handle(CreateChargeStationCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var group = await _groupRepository.GetGroupById(command.GroupId);
            if (group == null)
            {
                throw new ArgumentException($"A Group with Id {command.GroupId} does not exists.");
            }

            var chargeStationName = command.Name.Trim();
            var chargeStationNameExist = await _chargeStationRepository.IsNameExist(chargeStationName);
            if (chargeStationNameExist)
            {
                throw new ArgumentException($"A ChargeStation with the name {chargeStationName} already exists.");
            }
        
            var chargeStation = ChargeStationEntity.Create(chargeStationName);

            if (command.Connectors is null || command.Connectors.Count is 0)
            {
                throw new ArgumentException($"A ChargeStation name {chargeStationName} do not have connector.");
            } 
            
            foreach (var connectorRequest in command.Connectors)
            {
                var connector = ConnectorEntity.Create(connectorRequest.Name, connectorRequest.MaxCapacityInAmps);
                chargeStation.AddConnector(connector);
            }
            
            group.AddChargeStation(chargeStation);
            group.UpdateCapacity();
            
            await _chargeStationRepository.AddChargeStation(chargeStation);

            await _unitOfWork.SaveChangesAsync();
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