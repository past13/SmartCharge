using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Connector;

public class CreateConnectorHandler : IRequestHandler<CreateConnectorCommand, Result<ConnectorEntity>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    public CreateConnectorHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IGroupRepository groupRepository,
        IChargeStationRepository chargeStationRepository,
        IConnectorRepository connectorRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _groupRepository = groupRepository;
        _chargeStationRepository = chargeStationRepository;
        _connectorRepository = connectorRepository;
    }
    
    public async Task<Result<ConnectorEntity>> Handle(CreateConnectorCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var connectorName = command.Name.Trim();
            var connectorNameExist = await _connectorRepository.IsNameExist(connectorName);
            if (connectorNameExist)
            {
                throw new ArgumentException($"A Connector with the name {connectorName} already exists.");
            }
            
            var isChargeStationExist = await _chargeStationRepository.IsChargeStationExist(command.ChargeStationId);
            if (!isChargeStationExist)
            {
                throw new ArgumentException($"A ChargeStation with Id {command.ChargeStationId} does not exists.");
            }

            var group = await _groupRepository.GetGroupByChargeStationId(command.ChargeStationId);
            if (group is null)
            {
                throw new ArgumentException($"A Group does not exists.");
            }
            
            var chargeStation = group.ChargeStations.First(cs => cs.Id == command.ChargeStationId);
            
            var connector = ConnectorEntity.Create(connectorName, command.CapacityInAmps);
            chargeStation.AddConnector(connector);
            
            group.UpdateCapacity();

            await _connectorRepository.AddConnector(connector);
            
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<ConnectorEntity>.Success(null);
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure(ex.Message);
        }
    }
}